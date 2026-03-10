using RPG.Combat;
using RPG.Movement;
using RPG.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Health _health;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType _type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] _cursorMappings = null;
        [SerializeField] float _maxNavMeshProjectionDistance = 1f;
        [SerializeField] float _raycastRadius = 1f;

        private void Awake()
        {
           _health = GetComponent<Health>();
        }


        private void Update()
        {
            if (InteractWithUI()) return;
            if (_health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithComponent()) return;
            if (InteractWithMovement())return;
            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] _hits = RaycastAllSorted();
            foreach (RaycastHit _hit in _hits)
            {
                IRaycastable[] _raycastables = _hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable _raycastable in _raycastables)
                {
                    if(_raycastable.HandleRaycast(this))
                    {
                        SetCursor(_raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[]  RaycastAllSorted()
        {
            RaycastHit[] _hits = Physics.SphereCastAll(GetMouseRay(), _raycastRadius);
            float[] _distances = new float[_hits.Length];
            for(int i = 0; i < _hits.Length; i++)
            {
                _distances[i] = _hits[i].distance;
            }
            Array.Sort(_distances, _hits);
            return _hits;
        }

        private bool InteractWithUI()
        {
            if( EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

       
        
        private bool InteractWithMovement()
        {
            
           // RaycastHit _hit;
            //bool _hasHit = Physics.Raycast(GetMouseRay(), out _hit);

            Vector3 _target;
            bool _hasHit = RaycastNavMesh(out _target);
            if (_hasHit)
            {
                if (!GetComponent<Mover>().CanMoveTo(_target)) return false;
               
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(_target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 _target)
        {
            _target = new Vector3();

            RaycastHit _hit;
            bool _hasHit = Physics.Raycast(GetMouseRay(), out _hit);
            if (!_hasHit) return false;

            NavMeshHit _navMeshHit;
            bool _hasCastToNavMesh = NavMesh.SamplePosition(
                _hit.point, out _navMeshHit, _maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!_hasCastToNavMesh) return false;

            _target = _navMeshHit.position;

            

            return true;
        }

        

        private void SetCursor(CursorType _type)
        {
            CursorMapping _mapping = GetCursorMapping(_type);
            Cursor.SetCursor(_mapping.texture, _mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType _type)
        {
            foreach (CursorMapping _mapping in _cursorMappings)
            {
                if(_mapping._type == _type)
                {
                    return _mapping;
                }
            }
            return _cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
