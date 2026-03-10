using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {

        [SerializeField] DamageText _damageTextPrefab = null;

       
       public void Spawn(float _damageAmount)
        {
            DamageText _instance = Instantiate<DamageText>(_damageTextPrefab, transform);
            _instance.SetValue(_damageAmount);
        }
    }

}