﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;
using RPG.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenratePercentage = 70;
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
        }

       LazyValue<float> healthPoints;
       bool isDead = false;


        private void Awake()
        {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }
        private void Start()
        {
            healthPoints.ForceInit();
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health); 
        }

        public void Heal(float healthToRestore)
        {
            healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetMaxHealthPoints());
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;

        }

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject instigator,float damage)
        {
            print(gameObject.name + "  took damage of " + damage);
          
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            takeDamage.Invoke(damage);


            if (healthPoints.value == 0)
            {
                onDie.Invoke();
                Die();
                AwardExeperince(instigator);
            }
        }

        private void AwardExeperince(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null)
            {
                return;
            }
            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        public float GetHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPrecentage()
        {
           
            return GetFraction() * 100;
        }

        public float GetFraction()
        {

            return (healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        public void RegenerateHealth()
        {
            float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenratePercentage / 100);
            healthPoints.value = Mathf.Max(healthPoints.value, regenHealthPoints);
        }

        private void Die()
        {
            if (isDead)
            {
                return;
            }

            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();

        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;

            if (healthPoints.value <= 0)
            {
                Die();
            }
        }
    }
}
