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

        int currentLevel = 0;

        private void Start()
        {
            currentLevel = CalculateLevel();
        }

        private void Update()
        {  
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel)
            {
                currentLevel = newLevel;
                print("Levelled Up!");
            }
        }

        public float GetStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, CalculateLevel());
        }

        public int GetLevel()
        {
            return currentLevel;
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
