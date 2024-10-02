using System.Collections.Generic;
using FastFoodRush.Interactable;
using FastFoodRush.Manager;
using FastFoodRush.UI;
using UnityEngine;

namespace FastFoodRush.Object
{
    public abstract class BaseCounterTable : UnlockableObject
    {
        [SerializeField] protected ObjectStack _objectStack;
        [SerializeField] protected List<Transform> _queuePointList = new();
        [SerializeField] protected Transform _customerSpawnPoint;
        [SerializeField] protected Transform _customerDesapwnPoint;
        [SerializeField] protected float _spawnCustomerInterval = 3;
        [SerializeField] protected OrderInfoUI _orderInfoUI;
        [SerializeField] private OrderInfoType _orderInfoType;
        [SerializeField] private MoneyPile _moneyPile;
        [SerializeField] private WorkingSpot _workingSpot;
        [SerializeField] private GameObject _workerObj;
        
        protected Queue<IOrderable> _customerQueue = new();
        protected float _spawnCustomerElapsed;
        protected float _orderElapsed;
        protected Camera _camera;

        protected override void Start()
        {
            base.Start();
            _camera = Camera.main;
        }

        protected virtual void Update()
        {
            HandleSpawnCustomer();
            HandleOrder();
        }
        
        protected override void UpgradeableMesh()
        {
            base.UpgradeableMesh();
            
            if (_workerObj)
            {
                _workerObj.SetActive(_unlockLevel >= 2);
            }
        }

        protected virtual void HandleSpawnCustomer()
        {
            if (IsMaxQueuePoint())
            {
                return;
            }
            
            _spawnCustomerElapsed += Time.deltaTime;
            if (_spawnCustomerElapsed > _spawnCustomerInterval)
            {
                _spawnCustomerElapsed = 0;
                SpawnCustomer(_customerSpawnPoint.position, GetQueuePoint(), _customerDesapwnPoint.position);
            }
        }
        
        protected virtual void HandleOrder()
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

            HandleOrderInfoUI();
            if (_objectStack.StackCount > 0 && customer.OrderCount > customer.Height && _workingSpot.IsAvailableHandleOrder)
            {
                _orderElapsed += Time.deltaTime;
                if (_orderElapsed < 0.2f)
                {
                    return;
                }

                _orderElapsed = 0;
                bool isSuccessReceivedOrder = customer.TryReceivedOrderInfo(_objectStack.GetStackObject());
                if (isSuccessReceivedOrder)
                {
                    ReceivedTip();
                }
            }
            
            if (customer.RemainOrderCount == 0)
            {
                CompleteOrder();
            }
        }

        protected void ReceivedTip()
        {
            /*
             *  햄버거 한개 당 가격
             *  팁 = 음식 한개의 가격 + profit level * profit value
             */

            int price = RestaurantManager.Instance.GetPriceOfFood(_orderInfoType);
            _moneyPile.AddMoney(price);
        }
        
        protected void UpdateCustomerQueuePosition()
        {
            using var enumerator = _customerQueue.GetEnumerator();
            int index = 0;
            while (enumerator.MoveNext())
            {
                IOrderable current = enumerator.Current;
                if (current != null)
                {
                    current.UpdateQueuePosition(GetQueuePoint(index).position);
                    index++;
                }
            }
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
        
        private bool IsMaxQueuePoint()
        {
            return _customerQueue.Count >= _queuePointList.Count;
        }

        protected abstract void HandleOrderInfoUI();
        protected abstract void CompleteOrder();
        protected abstract void SpawnCustomer(Vector3 spawnPosition, Transform queuePoint, Vector3 despawnPosition);
    }
}