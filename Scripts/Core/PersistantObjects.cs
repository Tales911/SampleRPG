using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistantObjects : MonoBehaviour
    {
        [SerializeField] GameObject _persistantObjectPrefab;

        static bool _hasSpawned = false;

        private void Awake()
        {
            if (_hasSpawned) return;

            SpawnPersitantObject();

            _hasSpawned = true;
        }

        private void SpawnPersitantObject()
        {
            GameObject _persistantObject = Instantiate(_persistantObjectPrefab);
            DontDestroyOnLoad(_persistantObject);
        }

        
    }

}