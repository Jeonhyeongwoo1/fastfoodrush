using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FastFoodRush.Manager;
using FastFoodRush.Object;
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
        [SerializeField] private MainTutorialType _mainTutorialType;
        
        private Stack<GameObject> _objectStack;
        private int _currentSeatedCustomerCount;
        private int _remainSeatableChairCount;
        private int _tipChance;
        private float _eatingMinTime;
        private float _eatingMaxTime;
        
        private Action<int> onAllCustomerSeatedAction;
        private Action onLeaveCustomerAction;
        
        public override void Unlock(bool animate = true)
        {
            base.Unlock(animate);
            
            SetData();
        }

        private void SetData()
        {
            _tipChance = Const.TipChance + (_unlockLevel - 1) * 10;

            _eatingMinTime = 1f * (1 - (_unlockLevel - 1) * 0.2f);
            _eatingMaxTime = 3f * (1 - (_unlockLevel - 1) * 0.3f);
            Debug.LogWarning($"min {_eatingMinTime} / max {_eatingMaxTime}");
        }

        public void CompleteStackFood(Action<int> onAllCustomerSeatedAction, Action onLeaveCustomerAction)
        {
            this.onAllCustomerSeatedAction += onAllCustomerSeatedAction;
            this.onLeaveCustomerAction += onLeaveCustomerAction;
            _currentSeatedCustomerCount++;
            
            if (_currentSeatedCustomerCount == _chairList.Count)
            {
                this.onAllCustomerSeatedAction?.Invoke(_unlockLevel);
                StartCoroutine(BeginEatingCor());
            }
        }

        public void ReceivedTip()
        {
            int random = Random.Range(0, 100);
            if (random < _tipChance)
            {
                int tipMoney = RestaurantManager.Instance.GetTipAmount();
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
                yield return new WaitForSeconds(Random.Range(_eatingMinTime, _eatingMaxTime));
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

        public override void MainTutorialProgress()
        {
            TutorialManager tutorialManager = TutorialManager.Instance;
            bool isExecutedTutorial =  tutorialManager.CheckExecutedMainTutorialProgress(_mainTutorialType);
            if (!isExecutedTutorial)
            {
                tutorialManager.SetTutorialTarget(RestaurantManager.Instance.UnlockableBuyer.transform);
                tutorialManager.LoadTutorial(_mainTutorialType);
            }
        }
    }
}