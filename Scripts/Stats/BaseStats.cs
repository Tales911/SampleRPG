using Dev.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int _startingLevel = 1;
        [SerializeField] CharacterClass _characterClass;
        [SerializeField] Progression _progression = null;
        [SerializeField] GameObject _levelUpFX = null;

        [SerializeField] bool _shouldUseModifiers = false;

        public event Action _onLevelUp;

        LazyValue<int> _currentLevel;

        Experience _experience;

        void Awake()
        {
            Experience _experience = GetComponent<Experience>();
            _currentLevel = new LazyValue<int>(CalculateLevel);
        }

        void Start()
        {
            _currentLevel.ForceInit();
        }

        void OnEnable()
        {
            if (_experience != null)
            {
                _experience._onExperienceGained += UpdateLevel;
            }
        }

        void OnDisable()
        {
            if (_experience != null)
            {
                _experience._onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int _newLevel = CalculateLevel();
            if (_newLevel > _currentLevel.value)
            {
                _currentLevel.value = _newLevel;
                print("levelled up");
                LevelUpEffect();
                _onLevelUp();
            }  
        }

        private void LevelUpEffect()
        {
            Instantiate(_levelUpFX, transform);
        }

        

        public float GetStat(Stat _stat)
        {
            return (GetBaseStat(_stat) + GetAdditiveModifiers(_stat)) * (1 + GetPercentageModifiers(_stat)/100);
        }

        

        private float GetBaseStat(Stat _stat)
        {
            return _progression.GetStat(_stat, _characterClass, GetLevel());
        }


        public int GetLevel()
        {
            return _currentLevel.value;
        }


        private int CalculateLevel()
        {
           Experience _experience = GetComponent<Experience>();
           if (_experience == null) return _startingLevel;

           float _currentXP = _experience.GetPoints();
           int _penultimateLevel = _progression.GetLevels(Stat.ExperienceToLevelUp, _characterClass);
           for (int _level = 1; _level <= _penultimateLevel; _level++)
            {
                float XPToLevelUp = _progression.GetStat(Stat.ExperienceToLevelUp, _characterClass, _level);
                if (XPToLevelUp > _currentXP)
                {
                    return _level;
                }
            }

            return _penultimateLevel + 1;
        }

        private float GetAdditiveModifiers(Stat _stat)
        {
            if (!_shouldUseModifiers) return 0;
            float _total = 0;
            foreach (IModifierProvider _provider in GetComponents<IModifierProvider>())
            {
                foreach (float _modifier in _provider.GetAdditiveModifiers(_stat)) 
                {
                    _total += _modifier;
                }
            }
            return _total;
        }

        private float GetPercentageModifiers(Stat _stat)
        {
            if (!_shouldUseModifiers) return 0;
            float _total = 0;
            foreach (IModifierProvider _provider in GetComponents<IModifierProvider>())
            {
                foreach (float _modifier in _provider.GetPercentageModifiers(_stat))
                {
                    _total += _modifier;
                }
            }
            return _total;
        }
    }
}