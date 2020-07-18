using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Resources;

using System;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health health;

        private void Awake()
        {
            health = GetComponent<Health>();   
        }

        private void Update()
        {

            if (health.IsDead())
            {
                return;
            }

            if (InteractWithCombat())
            {
                return;
            }
            if (InteractWithMovement())
            {
                return;
            }

            
            print("Nothing to do.");
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            foreach (RaycastHit hit in hits)
            {

                CombatTarget target =  hit.transform.GetComponent<CombatTarget>() as CombatTarget;

               
                
                if (target == null)
                {
                
                    return false;
                }
               
                if (GetComponent<Fighter>().CanAttack(target.gameObject))
                {
                    if (Input.GetMouseButton(0))
                    {
                      
                        GetComponent<Fighter>().Attack(target.gameObject);
                      
                    }
                    return true;
                }
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            
            RaycastHit hit;
            //Check if the ray has hit somthing
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(hit.point,1f);
                }
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {

            //Shoot a ray from the main camera 
            Ray lastRay =  Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(lastRay.origin, lastRay.direction * 100);
            return lastRay;
        }
    }
}

