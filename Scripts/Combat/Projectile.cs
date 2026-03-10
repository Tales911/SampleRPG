using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {

        [SerializeField] bool _isHoming = false;
        [SerializeField] float _projectileSpeed = 60f;
        [SerializeField] GameObject _hitEffect = null; //make an effect where the impact took place.
        [SerializeField] float _maxLifeTime = 20f; //max lifetime of projectile flying through air.
        [SerializeField] GameObject[] _destroyOnHit = null;
        [SerializeField] float _lifeAfterImpact = 3f; //the lifetime of the impact effect.
        [SerializeField] UnityEvent _onHit;

        Health _target = null;
        GameObject _instigator = null;
        float _damage = 0f;



        void Start()
        {
            transform.LookAt(GetAimLocation()); // i like this because you could have an enemy that runs and you could miss it.
        }


        void Update()
        {
            if (_target == null) return;
            if (_isHoming && !_target.IsDead()) //we could have a fun homing missle launcher for New Game++ unlocks
            {
                transform.LookAt(GetAimLocation());
            }
            transform.Translate(Vector3.forward * _projectileSpeed * Time.deltaTime);
        }

        public void SetTarget(Health _target, GameObject _instigator, float _damage)
        {
            this._target = _target;
            this._damage = _damage;
            this._instigator = _instigator;
            Destroy(gameObject, _maxLifeTime); //destroy stray projectile after a set amount of time if it hits nothing
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider _targetCapsuleC = _target.GetComponent<CapsuleCollider>();
            if (_targetCapsuleC == null)
            {
                return _target.transform.position;
            }
            return _target.transform.position + Vector3.up * _targetCapsuleC.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != _target) return;
            if (_target.IsDead()) return;
            _target.TakeDamage(_instigator, _damage);

            _projectileSpeed = 0;

            _onHit.Invoke();

            if (_hitEffect != null)
            {
                Instantiate(_hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject _toDestroy in _destroyOnHit)
            {
                Destroy(_toDestroy);
            }

            Destroy(gameObject, _lifeAfterImpact);
        }
    }

}