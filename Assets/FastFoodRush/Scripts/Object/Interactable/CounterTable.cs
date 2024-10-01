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
            GameObject obj = PoolManager.Instance.Get(Key.Customer);
            if (!obj.TryGetComponent(out CustomerController customerAI))
            {
                Debug.LogWarning($"failed get customer");
                return;
            }

            int maxFoodCapacity = Random.Range(1, 5);
            customerAI.Spawn(spawnPosition, queuePoint.position, maxFoodCapacity, despawnPosition);
            _customerQueue.Enqueue(customerAI);
        }
    }
}