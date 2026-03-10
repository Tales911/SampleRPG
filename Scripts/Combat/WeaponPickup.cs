using RPG.Attributes;
using RPG.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig _weapon = null;
        [SerializeField] float _healthToRestore = 0;
        [SerializeField] float _respawnTimer = 10f;

       
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject _subject)
        {
           if(_weapon != null)
            {
                _subject.GetComponent<Fighter>().EquipWeapon(_weapon);
            }
            if(_healthToRestore > 0)
            {
                _subject.GetComponent<Health>().Heal(_healthToRestore);
            }
            StartCoroutine(HideForSeconds(_respawnTimer));
        }

        private IEnumerator HideForSeconds(float _seconds)
        {
            ShowPickup(false);
            yield return new WaitForSeconds(_seconds);
            ShowPickup(true);
            
        }

        private void ShowPickup(bool _shouldShow)
        {
            GetComponent<Collider>().enabled = _shouldShow;
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(_shouldShow);
            }
        }

       public bool HandleRaycast(PlayerController _callingController)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Pickup(_callingController.gameObject);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup; 
        }
    }

}
