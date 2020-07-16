using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience experience;

        private void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            UpdateExperienceText();
        }

        private void UpdateExperienceText()
        {
            GetComponent<Text>().text = experience.GetExperience().ToString();
        }
    }
}
