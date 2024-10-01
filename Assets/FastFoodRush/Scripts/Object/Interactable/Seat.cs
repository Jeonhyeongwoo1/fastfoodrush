using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FastFoodRush.Manager;
using FastFoodRush.Object;
using FastFoodRush.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FastFoodRush.Interactable
{
    public class Seat : UnlockableObject
    {
        public int RemainSeatableChairCount => _chairList.Count - _remainSeatableChairCount;
        
        [SerializeField] private List<Transform> _chairList;
        [SerializeField] private Transform _table;
        [SerializeField] private Transform _tableTop;
        [SerializeField] private TrashPile _trashPile;
        [SerializeField] private MoneyPile _tipMoneyPile;
        
        private Stack<GameObject> _objectStack;
        private int _currentSeatedCustomerCount;
        private int _remainSeatableChairCount;
        private int _tipChance;
        
        private Action onAllCustomerSeatedAction;
        private Action onLeaveCustomerAction;

        private void Start()
        {
            _tipChance = Const.TipChance + (_unlockLevel - 1) * 10;
        }

        public void CompleteStackFood(Action onAllCustomerSeatedAction, Action onLeaveCustomerAction)
        {
            this.onAllCustomerSeatedAction += onAllCustomerSeatedAction;
            this.onLeaveCustomerAction += onLeaveCustomerAction;
            _currentSeatedCustomerCount++;
            
            if (_currentSeatedCustomerCount == _chairList.Count)
            {
                this.onAllCustomerSeatedAction?.Invoke();
                StartCoroutine(BeginEatingCor());
            }
        }

        public void ReceivedTip()
        {
            int random = Random.Range(0, 100);
            if (random < _tipChance)
            {
                int tipMoney = RestaurantManager.Instance.GetTipAmount();
                Debug.Log("Tip" + tipMoney);
                _tipMoneyPile.AddMoney(tipMoney);    
            }
        }

        public Vector3 GetTablePosition()
        {
            return _table.position;
        }

        private IEnumerator BeginEatingCor()
        {
            int trashCount = 0;
            while (_objectStack.Count > 0)
            {
                yield return new WaitForSeconds(Random.Range(0.8f, 1.5f));
                GameObject obj = _objectStack.Pop();
                obj.SetActive(false);
                trashCount++;
            }
            
            ReceivedTip();
            onLeaveCustomerAction?.Invoke();
            CreateTrashPile(trashCount);
            Reset();
        }

        private void CreateTrashPile(int trashCount)
        {
            for (int i = 0; i < trashCount; i++)
            {
                _trashPile.Drop();
            }
        }

        private void Reset()
        {
            _currentSeatedCustomerCount = 0;
            _remainSeatableChairCount = 0;
            onAllCustomerSeatedAction = null;
            onLeaveCustomerAction = null;
        }
        
        public Vector3 GetChairPosition()
        {
            if (!IsPossibleSeat())
            {
                Debug.LogWarning("is full seat");
                return Vector3.zero;
            }
            
            return _chairList[_remainSeatableChairCount++].position;
        }

        public void StackFood(GameObject obj, float duration)
        {
            _objectStack ??= new Stack<GameObject>();
            Vector3 endValue = _tableTop.position + new Vector3(0, 0.25f, 0) * _objectStack.Count;
            _objectStack.Push(obj);
            obj.transform.DOJump(endValue, 2, 1, duration);
        }

        public bool IsPossibleSeat()
        {
            return _chairList.Count > _remainSeatableChairCount && !_trashPile.IsExistObject;
        }
    }
}