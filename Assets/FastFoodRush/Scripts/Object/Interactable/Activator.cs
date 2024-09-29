using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class Activator : Interactable
    {
        [SerializeField] private GameObject _activateObj;
        
        protected override void OnPlayerEnter(Transform tr)
        {
            OnActivate();
        }

        protected override void OnPlayerExit(Transform tr)
        {
            OnDeactivate();
        }
        
        private void OnActivate()
        {
            _activateObj.SetActive(true);    
        }

        private void OnDeactivate()
        {
            _activateObj.SetActive(false);
        }
    }
}
