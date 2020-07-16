using RPG.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using RPG.Resources;


namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {

        Fighter fighter;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateHealthText();
        }

        private void UpdateHealthText()
        {  
            if (fighter.GetTarget() != null)
            {
                GetComponent<Text>().text = String.Format("{0:0}/{1:0}", 
                    fighter.GetTarget().GetHealthPoints(), fighter.GetTarget().GetMaxHealthPoints());
            }
            else
            {
                GetComponent<Text>().text = "N/A";
            }
        }
    }
}
