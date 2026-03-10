using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {

        const string _defaultSaveFile = "Save";
        [SerializeField] float _fadeInTime = 0.2f;

        void Awake()
        {
            StartCoroutine(LoadLastScene());
        }
        
        IEnumerator LoadLastScene()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(_defaultSaveFile);
            Fader _fader = FindObjectOfType<Fader>();
            _fader.FadeOutImmediate();
            yield return _fader.FadeIn(_fadeInTime);
        }


        void Update()
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if(Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Delete();
            }
        }

 
        public void Load()
        {

            GetComponent<SavingSystem>().Load(_defaultSaveFile);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(_defaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(_defaultSaveFile);
        }
    }
}
