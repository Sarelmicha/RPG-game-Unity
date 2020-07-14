﻿using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] GameObject equippedPrefab = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;
        

        const string WEAPON_NAME = "Weapon";


        public void Spawn(Transform rightHand,Transform leftHand, Animator animator)
        {

            //Destory old weapon before switch to the new pickup weapon
            DestroyOldWeapon(rightHand,leftHand);

            if (equippedPrefab != null)
            {
               Transform handTransform = GetTransform(rightHand, leftHand);
               GameObject weapon =  Instantiate(equippedPrefab, handTransform);
               weapon.name = WEAPON_NAME;
            }

            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
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

        public void LaunchProjecilte(Transform rightHand, Transform leftHand, Health target)     
        {
            
            Projectile projectileInstance = Instantiate(projectile, 
                GetTransform(rightHand, leftHand).position, 
                Quaternion.identity);

            projectileInstance.SetTarget(target, weaponDamage);
        }
    }
}
