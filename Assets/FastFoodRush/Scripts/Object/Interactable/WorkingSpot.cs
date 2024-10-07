using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Manager;
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

            if (TutorialManager.Instance.CheckExecutedMainTutorialProgress(MainTutorialType.FoodMachine))
            {
                StartCoroutine(CheckFirstSeatTutorialCor());
            }
        }

        private IEnumerator CheckFirstSeatTutorialCor()
        {
            yield return new WaitForSeconds(2f);
            
            TutorialManager.Instance.CompleteMainTutorialDepth(MainTutorialType.FirstSeat);
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
