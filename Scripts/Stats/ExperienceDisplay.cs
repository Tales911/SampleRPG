using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
       //This is a Debug health display
        
        Experience _experience;
  
        private void Awake()
        {
            _experience = GameObject.FindWithTag("Player").GetComponent<Experience>();  
        }


        void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}", _experience.GetPoints());
            
        }
    }
}
