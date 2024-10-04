using System.Collections.Generic;
using System.Linq;
using FastFoodRush.Manager;
using FastFoodRush.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FastFoodRush.Object
{
    public class DriveThruCounterTable : BaseCounterTable
    {
        private readonly int driverTypeCount = 4;

        protected override void HandleOrderInfoUI()
        {            
            var customer = _customerQueue.Peek();
            if (_camera != null)
            {
                if (customer.RemainOrderCount > 0)
                {
                    Vector3 screenPoint = _camera.WorldToScreenPoint(customer.Transform.position + Vector3.up * 3);
                    _orderInfoUI.Show(customer.RemainOrderCount.ToString(), (int)OrderInfoType.Package, screenPoint);
                }
                else
                {
                    _orderInfoUI.Hide();
                }
            }
        }

        protected override void CompleteOrder()
        {
            var orderable = _customerQueue.Dequeue();
            if (orderable.Transform.TryGetComponent(out Driver driver))
            {
                driver.Leave();
            }
            
            UpdateCustomerQueuePosition();
        }

        protected override void SpawnCustomer(Vector3 spawnPosition, Transform queuePoint, Vector3 despawnPosition)
        {
            int random = Random.Range(1, driverTypeCount + 1);
            string key = $"Car0{random}";
            GameObject obj = PoolManager.Instance.Get(key);
            if (!obj.TryGetComponent(out Driver driver))
            {
                Debug.LogWarning($"failed get customer");
                return;
            }

            List<Vector3> movePositionList = new();
            bool isGet = false;
            for (int i = 0; i < _queuePointList.Count; i++)
            {
                Transform tr = _queuePointList[i];
                if (queuePoint == tr)
                {
                    isGet = true;
                }

                if (isGet)
                {
                    movePositionList.Add(tr.position);
                }
            }
            
            int maxFoodCapacity = Random.Range(1, 5);
            driver.Spawn(spawnPosition, 1, despawnPosition, movePositionList);
            _customerQueue.Enqueue(driver);
        }

        public override void MainTutorialProgress()
        {
           
            TutorialManager tutorialManager = TutorialManager.Instance;
            bool isExecutedTutorial =  tutorialManager.CheckExecutedMainTutorialProgress(MainTutorialType.DriveThruCounterTable);
            if (!isExecutedTutorial)
            {
                tutorialManager.SetTutorialTarget(RestaurantManager.Instance.UnlockableBuyer.transform);
                tutorialManager.LoadTutorial(MainTutorialType.DriveThruCounterTable);
            }
        }
    }
}