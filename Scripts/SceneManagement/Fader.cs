using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        //When Scene loads in and out, we fade to or from black
        
        CanvasGroup _canvasGroup;
        Coroutine _currentActiveFade = null;
       
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1;
        }

        public Coroutine FadeOut(float _time)
        {
            return Fade(1, _time);
        }

        
        public Coroutine FadeIn(float _time)
        {
            return Fade(0, _time);
        }

        public Coroutine Fade(float _target, float _time)
        {
            if (_currentActiveFade != null)
            {
                StopCoroutine(_currentActiveFade);
            }
            _currentActiveFade = StartCoroutine(FadeRoutine(_target, _time));
            return _currentActiveFade;
        }

        private IEnumerator FadeRoutine(float _target, float _time)
        {
            while (!Mathf.Approximately(_canvasGroup.alpha, _target))
            {
                _canvasGroup.alpha = Mathf.MoveTowards(_canvasGroup.alpha, _target, Time.deltaTime / _time);
                yield return null;
            }
        }
    }
}
    
