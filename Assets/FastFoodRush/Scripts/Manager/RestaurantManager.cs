using System;
using System.Collections.Generic;
using FastFoodRush.Object;
using UnityEngine;

namespace FastFoodRush.Manager
{
    public class RestaurantManager : MonoBehaviour
    {
        public static RestaurantManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<RestaurantManager>();
                }

                return instance;
            }
        }

        private static RestaurantManager instance;

        public int UnlockableObjectCount
        {
            get => _data.unlockableObjectCount;
            set => _data.unlockableObjectCount = value;
        }

        public int Moneny
        {
            get => _data.money;
            set
            {
                _data.money = value;
                onUseMoneny?.Invoke(_data.money);
            }
        }

        public int PaidAmount
        {
            get => _data.paidAmount;
            set => _data.paidAmount = value;
        }

        public Action<int> onUseMoneny;

        [SerializeField] private UnlockableBuyer _unlockableBuyer;
        [SerializeField] private List<UnlockableObject> _unlockableObjectList;
        
        private RestaurantData _data;
        
        private const int startMoneny = 500;
        
        private void Start()
        {
            _data = new RestaurantData();
            Moneny = startMoneny;
            AllDisableUnlockableObject();
            UnlockableObject unlockableObject = _unlockableObjectList[UnlockableObjectCount];
            // unlockableObject.Unlock();
            _unlockableBuyer.Initialize(unlockableObject, PaidAmount);
        }

        private void AllDisableUnlockableObject()
        {
            _unlockableObjectList.ForEach(v=> v.gameObject.SetActive(false));
        }

        public void BuyUnlockableObject()
        {
            if (_unlockableObjectList.Count <= UnlockableObjectCount)
            {
                Debug.LogWarning($"already opened all unlockable object");
                return;
            }

            UnlockableObject unlockableObject = _unlockableObjectList[UnlockableObjectCount];
            unlockableObject.Unlock();
            UnlockableObjectCount++;
            PaidAmount = 0;

            UnlockableObject nextUnlockableObject = _unlockableObjectList[UnlockableObjectCount];
            _unlockableBuyer.transform.position = nextUnlockableObject.GetBuyPoint;
            _unlockableBuyer.Initialize(nextUnlockableObject, PaidAmount);
        }
    }
}





























