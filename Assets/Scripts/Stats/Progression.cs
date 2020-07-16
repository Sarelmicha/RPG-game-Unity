
using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];
            if (levels.Length < level)
            {
                return 0;
            }
            return levels[level - 1];
        }

        public float[] GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();

            return lookupTable[characterClass][stat];
        }

        private void BuildLookup()
        {
            if (lookupTable != null)
            {
                return;
            }
            // Populate the lookup table dictionary
            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionClass in characterClasses)
            {
                var stateLookupTable = new Dictionary<Stat, float[]>();

                foreach (ProgressionStat progressionStat in progressionClass.stats)
                {
                    stateLookupTable[progressionStat.stat] = progressionStat.levels;
                }
                    lookupTable[progressionClass.characterClass] = stateLookupTable;
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
            //public float[] health;
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat; 
            public float[] levels;
        }
    }
}
