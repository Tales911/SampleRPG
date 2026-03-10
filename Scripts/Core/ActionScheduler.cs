using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class ActionScheduler : MonoBehaviour
    {
        IAction _currentAction;
        public void StartAction(IAction _action)
        {
            if (_currentAction == _action) return;
            if(_currentAction != null)
            {
                _currentAction.Cancel();
            }
            
            _currentAction = _action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }


        void Update()
        {

        }
    }

}
