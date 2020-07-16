﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Resources
{
    public class Experience : MonoBehaviour, ISaveable
    {

        [SerializeField] float experiencePoints = 0f;

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
        }

        public float GetExperience()
        {
            return experiencePoints;
        }

        public object CaptureState()
        {
            return experiencePoints;
        }
        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }

    }
}
