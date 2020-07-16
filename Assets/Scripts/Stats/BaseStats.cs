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

        private void Update()
        {
            if (gameObject.tag == "Player")
            {
                print(GetLevel());
            }

        }

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, startingLevel);
        }

        public int GetLevel()
        {
            float currentXP = GetComponent<Experience>().GetExperience();
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
