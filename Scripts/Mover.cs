using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;


namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform _target;
        Health _health;

        [SerializeField] float _maxSpeed = 6f;
        [SerializeField] float _maxNavPathLength = 40f;
        NavMeshAgent _navMeshAgent;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _health = GetComponent<Health>();
        }


        void Update()
        {
            _navMeshAgent.enabled = !_health.IsDead();
            
            UpdateAnimator();
        }


        public bool CanMoveTo(Vector3 _destination)
        {
            NavMeshPath _path = new NavMeshPath();
            bool _hasPath = NavMesh.CalculatePath(transform.position, _destination, NavMesh.AllAreas, _path);
            if (!_hasPath) return false;
            if (_path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(_path) > _maxNavPathLength) return false;
            return true;
        }

        public void MoveTo(Vector3 destination, float _speedFraction)
        {
            _navMeshAgent.destination = destination;
            _navMeshAgent.speed = _maxSpeed * Mathf.Clamp01(_speedFraction);
            _navMeshAgent.isStopped = false;
        }

        public void StartMoveAction(Vector3 destination, float _speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            
            MoveTo(destination, _speedFraction);
        }

        public void Cancel()
        {
            _navMeshAgent.isStopped = true;  
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = _navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity); //I wonder if this would work for tank controls
            float _speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", _speed);
        }

        public object CaptureState()
        {
           
           return new SerializableVector3(transform.position);
        }

        public void RestoreState(object _state)
        {

            SerializableVector3 position = (SerializableVector3)_state;
            _navMeshAgent.enabled = false;
            transform.position = position.ToVector();
            _navMeshAgent.enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private float GetPathLength(NavMeshPath _path)
        {
            float _total = 0;
            if (_path.corners.Length < 2) return _total;
            for (int i = 0; i < _path.corners.Length - 1; i++)
            {
                _total += Vector3.Distance(_path.corners[i], _path.corners[i + 1]);
            }

            return _total;
        }
    }


}