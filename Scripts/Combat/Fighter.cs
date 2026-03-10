using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using Dev.Utils;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISavable, IModifierProvider
    { 
        [SerializeField] float _timeBetweenAttacks = 1f;
        [SerializeField] WeaponConfig _defaultWeapon = null;
        [SerializeField] Transform _rightHandTransform = null;
        [SerializeField] Transform _leftHandTransform = null;
        
        WeaponConfig _currentWeaponConfig;

        Health _target;
        float _timeSinceLastAttack = Mathf.Infinity;
        LazyValue<Weapon> _currentWeapon;
       
        void Awake()
        {
            _currentWeaponConfig = _defaultWeapon;
            _currentWeapon = new LazyValue<Weapon>(SetUpDefaultWeapon);
        }

        private Weapon SetUpDefaultWeapon()
        {
            return AttachWeapon(_defaultWeapon);
        }

        void Start()
        {
           _currentWeapon.ForceInit();
        }


        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            if (_target == null) return;
            if (_target.IsDead()) return;
            
            if (!GetIsInRange(_target.transform))
            {
                GetComponent<Mover>().MoveTo(_target.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        public void EquipWeapon(WeaponConfig _weapon)
        {
            _currentWeaponConfig = _weapon;
           _currentWeapon.value = AttachWeapon(_weapon);
        }

        private Weapon AttachWeapon(WeaponConfig _weapon)
        {
            Animator _animator = GetComponent<Animator>();
            return _weapon.Spawn(_rightHandTransform, _leftHandTransform, _animator);
        }

        public Health GetTarget()
        {
            return _target;
        }

        private void AttackBehaviour()
        {
            transform.LookAt(_target.transform);
            if (_timeSinceLastAttack > _timeBetweenAttacks)
            {
                TriggerAttack();
                _timeSinceLastAttack = 0;    
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        //Animation Event
        void Hit()
        {
            if (_target == null)  return;

            float _damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (_currentWeapon.value != null)
            {
                _currentWeapon.value.OnHit();
            }

            if (_currentWeaponConfig.HasProjectile())
            {
                _currentWeaponConfig.LaunchProjectile(_rightHandTransform, _leftHandTransform, _target, gameObject, _damage);
            }
            else
            {
                
                _target.TakeDamage(gameObject, _damage);
            }
        }

        void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange(Transform _targetTransform)
        {
            return Vector3.Distance(transform.position, _targetTransform.position) < _currentWeaponConfig.GetRange();
        }

        public bool CanAttack(GameObject _combatTarget)
        {
            if (_combatTarget == null) { return false; }
            if (!GetComponent<Mover>().CanMoveTo(_combatTarget.transform.position) &&
                !GetIsInRange(_combatTarget.transform)) 
            { 
                return false; 
            }
            Health _targetToTest = _combatTarget.GetComponent<Health>();
            return _targetToTest != null && !_targetToTest.IsDead();
        }


        public void Attack(GameObject _combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            
            _target = _combatTarget.GetComponent<Health>();
        }
        public void Cancel()
        {
            StopAttack();
            _target = null;
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat _stat)
        {
            if(_stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat _stat)
        {
            if (_stat == Stat.Damage)
            {
                yield return _currentWeaponConfig.GetPercentageBonus();
            }
        }

        public object CaptureState() //resource saving folder
        {
            return _currentWeapon.value.name;
        }
       
        public void RestoreState(object state) //resource saving
        {
            string _weaponName = (string)state;
            WeaponConfig _weapon = Resources.Load<WeaponConfig>(_weaponName);
            EquipWeapon(_weapon);
        }

        
    }

    
}
