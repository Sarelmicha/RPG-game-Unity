using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;


namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] float weaponPercentageBonus = 0;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;
        
        const string WEAPON_NAME = "Weapon";

        public Weapon Spawn(Transform rightHand,Transform leftHand, Animator animator)
        {
            Weapon weapon = null;
            //Destory old weapon before switch to the new pickup weapon
            DestroyOldWeapon(rightHand,leftHand);

            if (equippedPrefab != null)
            {
               Transform handTransform = GetTransform(rightHand, leftHand);
               weapon =  Instantiate(equippedPrefab, handTransform);
               weapon.gameObject.name = WEAPON_NAME;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                //If we are here last animation was NOT the deafult animation - so we set it up to deafult;
                animator.runtimeAnimatorController = animatorOverride.runtimeAnimatorController;
            }

            return weapon;
            
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(WEAPON_NAME);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(WEAPON_NAME); 
            }

            if (oldWeapon == null)
            {
                return;
            }

            oldWeapon.name = "DESTROYING";
            //Destory old weapon
            Destroy(oldWeapon.gameObject);

        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded)
            {
                handTransform = rightHand;
            }
            else
            {
                handTransform = leftHand;
            }

            return handTransform;
        }

        public float GetRange()
        {
            return weaponRange;
        }

        public float GetDamage()
        {
            return weaponDamage;
        }

        public bool HasProjecile()
        {
            return projectile != null;
        }

        public float GetPerctenageBonus()
        {
            return weaponPercentageBonus;
        }

        public void LaunchProjecilte(Transform rightHand, Transform leftHand, Health target,GameObject instigator,float calculatedDamage)     
        {
            
            Projectile projectileInstance = Instantiate(projectile, 
                GetTransform(rightHand, leftHand).position, 
                Quaternion.identity);

            projectileInstance.SetTarget(target,instigator,  calculatedDamage);
        }
    }
}
