using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FastFoodRush.Manager;
using FastFoodRush.Object;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class Seat : UnlockableObject
    {
        public int RemainSeatableChairCount => _chairList.Count - _currentSeatedCustomerCount;
        
        [SerializeField] private List<Transform> _chairList;
        [SerializeField] private Transform _table;
        [SerializeField] private Transform _tableTop;
        [SerializeField] private TrashPile _trashPile;
        
        private Stack<GameObject> _objectStack;
        private int _currentSeatedCustomerCount;

        private Action onAllCustomerSeatedAction;
        private Action onLeaveCustomerAction;
        
        public void CompleteStackFood(Action onAllCustomerSeatedAction, Action onLeaveCustomerAction)
        {
            this.onAllCustomerSeatedAction += onAllCustomerSeatedAction;
            this.onLeaveCustomerAction += onLeaveCustomerAction;
            if (RemainSeatableChairCount == 0)
            {
                this.onAllCustomerSeatedAction?.Invoke();
                RemoveStackFood();
            }
        }

        public override void Unlock()
        {
            base.Unlock();
            
            RestaurantManager.Instance.Piles.Add(_trashPile);
        }

        public Vector3 GetTablePosition()
        {
            return _table.position;
        }

        private void RemoveStackFood()
        {
            StartCoroutine(RemoveStackFoodCor());
        }

        private IEnumerator RemoveStackFoodCor()
        {
            int count = _objectStack.Count;
            while (_objectStack.Count > 0)
            {
                yield return new WaitForSeconds(1.5f);
                Debug.LogWarning($"remove Stack {_objectStack.Count}");
                GameObject obj = _objectStack.Pop();
                obj.SetActive(false);
            }
            
            onLeaveCustomerAction?.Invoke();
            
            CreateTrashPile(count);
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
            
            return _chairList[_currentSeatedCustomerCount++].position;
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
            return _chairList.Count > _currentSeatedCustomerCount && !_trashPile.IsExistObject;
        }
    }
}