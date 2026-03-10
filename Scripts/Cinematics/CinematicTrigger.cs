using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics

{
    public class CinematicTrigger : MonoBehaviour
    {

        bool _alreadyTrigged = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!_alreadyTrigged && other.gameObject.tag == "Player")
            {
                _alreadyTrigged = true;
                GetComponent<PlayableDirector>().Play();
            }

        }
    }
}
