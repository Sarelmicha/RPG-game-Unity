using GameDevTV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifier = false;

        public event Action onLevelUp;

        LazyValue<int> currentLevel;
        Experience experience;


        private void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
            
        }

        private void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {  
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private float GetAdditiveModifier(Stat stat)
        {

            if (!shouldUseModifier)
            {
                return 0;
            }

            float total = 0;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat)+ GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100)  ;
        }

        private float GetPercentageModifier(Stat stat)
        {

            float total = 0;

            if (!shouldUseModifier)
            {
                return 0;
            }

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, CalculateLevel());
        }

        public int GetLevel()
        {
           
            return currentLevel.value;
        }

        public int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();

            if (experience == null)
            {
                return startingLevel;
            }

            float currentXP = experience.GetExperience();
            float[] levelsXP = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            return SearchCurrentLevel(levelsXP, currentXP);
        }

        private int SearchCurrentLevel(float[] levelsXP, float currentXP)
        {

            for (int level = 0; level < levelsXP.Length; level++)
            {
                if (currentXP < levelsXP[level])
                {
                    return level + 1;
                }
            }

            return levelsXP.Length + 1;
        }
    }      
}
