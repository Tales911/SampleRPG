using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] _characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> _lookupTable = null;

        public float GetStat(Stat _stat, CharacterClass _characterClass, int _level)
        {
            BuildLookup();

            float[] _levels = _lookupTable[_characterClass][_stat];

            if(_levels.Length < _level)
            {
                return 0;
            }

            return _levels[_level - 1];
            
        }

        public int GetLevels(Stat _stat, CharacterClass _characterClass)
        {
            BuildLookup();

            float[] _levels = _lookupTable[_characterClass][_stat];
            return _levels.Length;
        }

        private void BuildLookup()
        {
            if (_lookupTable != null) return;

            _lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass _progressionClass in _characterClasses)
            {
                var _statLookupTable = new Dictionary<Stat, float[]>();

                foreach (ProgressionStat _progressionStat in _progressionClass._stats)
                {
                    _statLookupTable[_progressionStat._stat] = _progressionStat._levels;
                }

                _lookupTable[_progressionClass._characterClass] = _statLookupTable;
            }

        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass _characterClass;
            public ProgressionStat[] _stats;  
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat _stat;
            public float[] _levels;
        }

    }
}