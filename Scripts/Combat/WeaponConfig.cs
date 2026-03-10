
using UnityEngine;
using RPG.Attributes;


namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController _animatorOverride = null;
        [SerializeField] Weapon _equippedWPNPrefab = null;
        [SerializeField] float _weaponRange = 2f;
        [SerializeField] float _weaponDamage = 5f;
        [SerializeField] float _weaponPercentageBonus = 0f;
        [SerializeField] bool _isRightHanded = true;
        [SerializeField] Projectile _projectile = null;

        const string _weaponName = "Weapon";

        public Weapon Spawn(Transform _rightHand, Transform _leftHand,  Animator _animator)
        {
            DestroyOldWeapon(_rightHand, _leftHand);

            Weapon _weapon = null;

            if (_equippedWPNPrefab != null)
            {
                Transform _handTransform = GetTransform(_rightHand, _leftHand);
                _weapon = Instantiate(_equippedWPNPrefab, _handTransform);
                _weapon.gameObject.name = _weaponName;
            }
            var _overrideController = _animator.runtimeAnimatorController as AnimatorOverrideController;
            if (_animatorOverride != null)
            {
                _animator.runtimeAnimatorController = _animatorOverride;
            }
            else if (_overrideController != null)
            {
                _animator.runtimeAnimatorController = _overrideController.runtimeAnimatorController; 
            }

            return _weapon;
        }

        private void DestroyOldWeapon(Transform _rightHand, Transform _leftHand)
        {
            Transform _oldWeapon = _rightHand.Find(_weaponName);
            if(_oldWeapon == null)
            {
                _oldWeapon = _leftHand.Find(_weaponName);
            }
            if (_oldWeapon == null) return;

            _oldWeapon.name = "DESTROYING";
            Destroy(_oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform _rightHand, Transform _leftHand)
        {
            Transform _handTransform;
            if (_isRightHanded) _handTransform = _rightHand;
            else _handTransform = _leftHand;
            return _handTransform;
        }

        public bool HasProjectile()
        {
            return _projectile != null;
        }

        public void LaunchProjectile(Transform _rightHand, Transform _leftHand, Health _target, GameObject _instigator, float _calculatedDamage)
        {
            Projectile _proectileInstance = Instantiate(_projectile, GetTransform(_rightHand, _leftHand).position, Quaternion.identity);
            _proectileInstance.SetTarget(_target, _instigator, _calculatedDamage);
        }

        public float GetDamage()
        {
            return _weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return _weaponPercentageBonus;
        }

        public float GetRange()
        {
            return _weaponRange;
        }
    }

}