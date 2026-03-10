using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using RPG.Attributes;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
       //This is a Debug health display
        
        Fighter _fighter;
  
        private void Awake()
        {
            _fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();  
        }


        void Start()
        {

        }


        void Update()
        {
            if(_fighter.GetTarget() == null)
            {
                GetComponent<Text>().text = "N/A";
                return;
            }
            Health _health = _fighter.GetTarget();
            GetComponent<Text>().text = String.Format("{0:0}/ {1:0}", _health.GetHealthPoints(), _health.GetMaxHealthPoints());
        }
    }
}
