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
    public class CounterTable : UnlockableObject
    {
        [SerializeField] private ObjectStack _objectStack;
        [SerializeField] private List<Transform> _queuePointList = new();
        [SerializeField] private Transform _customerSpawnPoint;
        [SerializeField] private Transform _customerDesapwnPoint;
        [SerializeField] private float _spawnCustomerInterval = 3;
        [SerializeField] private List<Seat> _seatList;
        
        private Queue<CustomerController> _customerQueue = new();
        private float _spawnCustomerElapsed;
        private float _orderElapsed;

        public override void Unlock()
        {
            base.Unlock();
            
            RestaurantManager.Instance.ObjectStacks.Add(_objectStack);
        }

        private void Update()
        {
            HandleSpawnCustomer();
            HandleOrder();
        }

        private void HandleOrder()
        {
            if (_customerQueue.Count == 0)
            {
                return;
            }

            var customer = _customerQueue.Peek();
            if (!customer.IsReadyOrder)
            {
                return;
            }

            _orderElapsed += Time.deltaTime;
            if (_objectStack.StackCount > 0 && customer.RemainOrderCount > 0)
            {
                if (_orderElapsed < 0.2f)
                {
                    return;
                }

                _orderElapsed = 0;
                customer.ReceiveOrderInfo(_objectStack.GetStackObject());
            }
            
            if (customer.RemainOrderCount == 0)
            {
                //Complete
                Seat seat = TryGetSeat();
                if (seat != null)
                {
                    var customerAI = _customerQueue.Dequeue();
                    customerAI.MoveToTable(seat.GetChairPosition(), seat);
                    UpdateCustomerQueuePosition();
                }
            }
            
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(customer.transform.position + Vector3.up * 3);
            RestaurantManager.Instance.onOrderProduct?.Invoke(customer.RemainOrderCount, (int) OrderInfoType.Food, screenPoint);
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

        private void UpdateCustomerQueuePosition()
        {
            using var enumerator = _customerQueue.GetEnumerator();
            int index = 0;
            while (enumerator.MoveNext())
            {
                CustomerController current = enumerator.Current;
                if (current != null)
                {
                    current.UpdateQueuePosition(GetQueuePoint(index).position);
                    index++;
                }
            }
        }

        private void HandleSpawnCustomer()
        {
            if (IsMaxQueuePoint())
            {
                return;
            }
            
            _spawnCustomerElapsed += Time.deltaTime;
            if (_spawnCustomerElapsed > _spawnCustomerInterval)
            {
                _spawnCustomerElapsed = 0;
                SpawnCustomer(_customerSpawnPoint.position, GetQueuePoint().position, _customerDesapwnPoint.position);
            }
        }

        private void SpawnCustomer(Vector3 spawnPosition, Vector3 queuePointPosition, Vector3 despawnPosition)
        {
            GameObject obj = PoolManager.Instance.Get(Key.Customer);
            if (!obj.TryGetComponent(out CustomerController customerAI))
            {
                Debug.LogWarning($"failed get customer");
                return;
            }

            int maxFoodCapacity = Random.Range(1, 5);
            customerAI.Spawn(spawnPosition, queuePointPosition, maxFoodCapacity, despawnPosition);
            _customerQueue.Enqueue(customerAI);
        }

        private bool IsMaxQueuePoint()
        {
            return _customerQueue.Count >= _queuePointList.Count;
        }

        private Transform GetQueuePoint(int index = -1)
        {
            if (IsMaxQueuePoint())
            {
                Debug.LogWarning($"max queue point count");
                return null;
            }

            Transform point = _queuePointList[index == -1 ? _customerQueue.Count : index];
            return point;
        }

        public Transform GetCustomerSpawnPoint()
        {
            return _customerSpawnPoint;
        }
    }
}