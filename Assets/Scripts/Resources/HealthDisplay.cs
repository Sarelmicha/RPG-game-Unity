using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace RPG.Resources
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;

        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();

        }

        private void Update()
        {
            UpdateHealthText();
        }

        private void UpdateHealthText()
        {
            GetComponent<Text>().text = String.Format("{0:0}%", health.GetPrecentage());
        }
    }
}
