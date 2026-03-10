using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using RPG.Saving;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float _experiencePoints = 0;

        public event Action _onExperienceGained;
        
        public void GainExperience(float _experience)
        {
            _experiencePoints += _experience;
            _onExperienceGained();
        }

        public float GetPoints()
        {
            return _experiencePoints;
        }
        
        public object CaptureState()
        {
            return _experiencePoints;
        }

        public void RestoreState(object _state)
        {
            _experiencePoints = (float)_state;
        }

    }
 
}