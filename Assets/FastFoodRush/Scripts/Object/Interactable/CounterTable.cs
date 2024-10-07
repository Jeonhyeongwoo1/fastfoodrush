using System;
using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Manager;
using FastFoodRush.Object;
using FastFoodRush.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FastFoodRush.Interactable
{
    public class CounterTable : BaseCounterTable
    {
        [SerializeField] private List<Seat> _seatList;

        protected override void CompleteOrder()
        {
            Seat seat = TryGetSeat();
            bool seatIsNotNull = seat != null;
            if (seatIsNotNull)
            {
                IOrderable iterable = _customerQueue.Dequeue();
                if (iterable.Transform.TryGetComponent(out CustomerController customerAI))
                {
                    customerAI.MoveToTable(seat.GetChairPosition(), seat);
                }

                UpdateCustomerQueuePosition();
            }
        }

        protected override void HandleOrderInfoUI()
        {
            Seat seat = TryGetSeat();
            bool seatIsNotNull = seat != null;
            var customer = _customerQueue.Peek();
            if (_camera != null)
            {
                if (customer.RemainOrderCount > 0)
                {
                    Vector3 screenPoint = _camera.WorldToScreenPoint(customer.Transform.position + Vector3.up * 3);
                    _orderInfoUI.Show(customer.RemainOrderCount.ToString(), (int)OrderInfoType.Food, screenPoint);
                }
                else  if (customer.RemainOrderCount == 0 && !seatIsNotNull)
                {
                    Vector3 screenPoint = _camera.WorldToScreenPoint(customer.Transform.position + Vector3.up * 3);
                    _orderInfoUI.Show(Const.NoSeat, (int)OrderInfoType.Food, screenPoint);
                }
                else
                {
                    _orderInfoUI.Hide();
                }
            }
        }

        private Seat TryGetSeat()
        {
            Seat selected = _seatList.Find(v => v.IsUnlock && v.IsPossibleSeat() && v.RemainSeatableChairCount > 0);
            if (selected != null)
            {
                return selected;
            }
            
            if (selected == null)
            {
                selected = _seatList.Find(v => v.IsUnlock && v.IsPossibleSeat() && v.RemainSeatableChairCount == 0);
                return selected ? selected : null;
            }

            return null;
        }

        protected override void SpawnCustomer(Vector3 spawnPosition, Transform queuePoint, Vector3 despawnPosition)
        {
            if (RestaurantManager.Instance.FoodMachineList.Count == 0)
            {
                return;
            }

            GameObject obj = PoolManager.Instance.Get(PoolKey.Customer);
            if (!obj.TryGetComponent(out CustomerController customerAI))
            {
                Debug.LogWarning($"failed get customer");
                return;
            }

            int min = 1;
            int max = 1;
            if (TutorialManager.Instance.MainTutorialStep > (int)MainTutorialType.FirstSeat && RestaurantManager.Instance.UnlockableObjectCount <= 7)
            {
                max = 5;
            }
            else if (RestaurantManager.Instance.UnlockableObjectCount > 7)
            {
                max = 8;
            }
            else
            {
                max = 3;
            }
            
            int maxFoodCapacity = Random.Range(min, max);
            customerAI.Spawn(spawnPosition, queuePoint.position, maxFoodCapacity, despawnPosition);
            _customerQueue.Enqueue(customerAI);
        }

        public override void LoadMainTutorial()
        {
            TutorialManager tutorialManager = TutorialManager.Instance;
            bool isExecutedTutorial =  tutorialManager.CheckExecutedMainTutorialProgress(MainTutorialType.RestaurantCountTable);
            if (!isExecutedTutorial)
            {
                tutorialManager.SetTutorialTarget(RestaurantManager.Instance.UnlockableBuyer.transform);
                tutorialManager.LoadTutorial(MainTutorialType.RestaurantCountTable);
            }
        }
        
        public override void CompleteMainTutorialProgress()
        {
            TutorialManager.Instance.CompleteMainTutorialDepth(MainTutorialType.RestaurantCountTable);
        }
    }
}