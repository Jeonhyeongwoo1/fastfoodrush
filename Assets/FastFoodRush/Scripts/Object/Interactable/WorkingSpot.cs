using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class WorkingSpot : Interactable
    {
        public bool IsAvailableHandleOrder => _player != null || _counterEmployeeObj.activeSelf;
        
        [SerializeField] private GameObject _counterEmployeeObj;
        [SerializeField] private Image _indicatorImage;
        
        protected override void OnPlayerEnter(Transform tr)
        {
            base.OnPlayerEnter(tr);
            _indicatorImage.color = Color.green;
        }

        protected override void OnPlayerExit(Transform tr)
        {
            base.OnPlayerExit(tr);
            _indicatorImage.color = Color.red;
        }

        public void ActivateCounterEmployee()
        {
            _counterEmployeeObj.SetActive(true);
        }
    }
}
