using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
       
        
        BaseStats _baseStats;
  
        private void Awake()
        {
            _baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();  
        }


        void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}", _baseStats.GetLevel());
            
        }
    }
}
