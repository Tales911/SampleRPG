using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Control;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E
        }

        [SerializeField] int _sceneToLoad = 1; //change scene number in editor by putting scene number from build index
        [SerializeField] Transform _spawnPoint;
        [SerializeField] DestinationIdentifier _destination;
        
        [SerializeField] float _timeToFadeOut = 2f;
        [SerializeField] float _timeToFadeIn = 2f;
        [SerializeField] float _timeToFadeWait = 0.5f;

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Player")
            {
                print("portal triggered");
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if(_sceneToLoad < 0)
            { 
                Debug.LogError("Scene to Load not set");
                yield break;
            }

            DontDestroyOnLoad(gameObject);

            Fader _fader = FindObjectOfType<Fader>();
            SavingWrapper _wrapper = FindObjectOfType<SavingWrapper>();
            PlayerController _playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            _playerController.enabled = false;

            yield return _fader.FadeOut(_timeToFadeOut);

            _wrapper.Save();

            yield return SceneManager.LoadSceneAsync(_sceneToLoad);
            PlayerController _newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            _newPlayerController.enabled = false;

            _wrapper.Load();

            Portal _otherPortal = GetOtherPortal();
            UpdatePlayer(_otherPortal);

            _wrapper.Save();

            yield return new WaitForSeconds(_timeToFadeWait);
            _fader.FadeIn(_timeToFadeIn);

            _newPlayerController.enabled = true;
            Destroy(gameObject); 
        }


        private Portal GetOtherPortal()
        {
            foreach (Portal _portal  in FindObjectsOfType<Portal>())
            {
                if (_portal == this) continue;
                if (_portal._destination != _destination) continue;

                return _portal;
            }

            return null;
        }
        private void UpdatePlayer(Portal _otherPortal)
        {
            GameObject _player = GameObject.FindWithTag("Player");
            _player.GetComponent<NavMeshAgent>().Warp(_otherPortal._spawnPoint.position);
            _player.transform.rotation = _otherPortal._spawnPoint.rotation;
        }

        
    }
}

