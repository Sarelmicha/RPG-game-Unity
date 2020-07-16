using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats stats;

        private void Awake()
        {
            stats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            UpdateLevelText();
        }

        private void UpdateLevelText()
        {
            GetComponent<Text>().text = stats.CalculateLevel().ToString();
        }
    }
}
