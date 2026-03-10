using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using System;
using Dev.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float _regenerationPercentage = 70f;
        [SerializeField] UnityEvent<float> _takeDamage;
        [SerializeField] UnityEvent _onDie;

        [SerializeField] LazyValue<float> _healthPoints;

        bool _isDead = false;

        void Awake()
        {
            _healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);

        }


        void Start()
        {
            _healthPoints.ForceInit();
            
        }

       

        private void OnEnable()
        {
            GetComponent<BaseStats>()._onLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>()._onLevelUp -= RegenerateHealth;
        }

        public bool IsDead()
        {
            return _isDead;
        }

        public void TakeDamage(GameObject _instagator, float _damage)
        {
            _healthPoints.value = Mathf.Max(_healthPoints.value - _damage, 0);
            
            if (_healthPoints.value == 0)
            {
                _onDie.Invoke();
                Die();
                AwardExperience(_instagator);
            }
            else
            {
                _takeDamage.Invoke(_damage);
            }
        }

        public void Heal(float _healthToRestore)
        {
            _healthPoints.value = Mathf.Min(_healthPoints.value + _healthToRestore, GetMaxHealthPoints());
        }


        public float GetHealthPoints()
        {
            return _healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        { 
            return _healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void RegenerateHealth()
        {
           float _regenHealthPoints =  _healthPoints.value = GetComponent<BaseStats>().GetStat(Stat.Health) * (_regenerationPercentage / 100);
           _healthPoints.value = Mathf.Max(_healthPoints.value, _regenHealthPoints);
        }


        private void Die()
        {
            if (_isDead) return;

            _isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
            
            //_enemyCollider.enabled = false; //temporary maybe
        }

        private void AwardExperience(GameObject _instigator)
        {
           Experience _experience = _instigator.GetComponent<Experience>();
            if (_experience == null) return;

            _experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        public object CaptureState()
        {
           return _healthPoints.value;
        }

        public void RestoreState(object state)
        {
            _healthPoints.value = (float)state;

            if (_healthPoints.value == 0)
            {
                Die();
            }
        }
    }

}
