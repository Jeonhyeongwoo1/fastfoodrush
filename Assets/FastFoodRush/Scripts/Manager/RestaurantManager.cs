using System;
using System.Collections.Generic;
using FastFoodRush.Object;
using FastFoodRush.Scripts.Data;
using UnityEngine;

namespace FastFoodRush.Manager
{
    public enum AbilityType
    {
        None,
        PlayerSpeed,
        PlayerCapacity,
        PlayerProfit,
        EmployeeSpeed,
        EmployeeCapacity,
        EmployeeAmount
    }
    
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
        public Action<int, int, Vector3> onOrderProduct;
        public Action<AbilityType> onUpgrade;

        [SerializeField] private UnlockableBuyer _unlockableBuyer;
        [SerializeField] private List<UnlockableObject> _unlockableObjectList;
        [SerializeField] private AbilityConfigData _abilityData;
        
        private RestaurantData _data;
        
        private const int startMoneny = 100000;
        
        private void Start()
        {
            _data = new RestaurantData();
            Moneny = startMoneny;
            AllDisableUnlockableObject();
            UnlockableObject unlockableObject = _unlockableObjectList[UnlockableObjectCount];
            _unlockableBuyer.Initialize(unlockableObject, PaidAmount, unlockableObject.GetBuyPointPosition, unlockableObject.GetBuyPointRotation);

            onUpgrade += OnAbilityUpgrade;
        }

        private void OnDestroy()
        {
            onUpgrade -= OnAbilityUpgrade;
        }

        private void OnAbilityUpgrade(AbilityType abilityType)
        {
            int currentMoney = Moneny;
            int price = GetCurrentAbilityPrice(abilityType);
            if (currentMoney < price)
            {
                Debug.LogWarning($"failed to upgrade ability current moneny {currentMoney} / price {price}");
                return;
            }

            Dictionary<int, int> dataDict = _data.abilityDataDict;
            if (!dataDict.TryGetValue((int)abilityType, out int level))
            {
                dataDict[(int)abilityType] = 1;
            }

            if (_abilityData.MaxLevel == level)
            {
                Debug.LogWarning("ability is max level");
                return;
            }

            dataDict[(int)abilityType] = ++level;
            //UseMoney
            Moneny -= price;
        }

        public int GetCurrentAbilityLevel(AbilityType abilityType)
        {
            Dictionary<int, int> dataDict = _data.abilityDataDict;
            if (dataDict.TryGetValue((int)abilityType, out int level))
            {
                return level;
            }
            
            level = 1;
            dataDict[(int)abilityType] = level;

            return level;
        }

        public int GetCurrentAbilityPrice(AbilityType abilityType)
        {
            Dictionary<int, int> dataDict = _data.abilityDataDict;
            if (!dataDict.TryGetValue((int)abilityType, out int level))
            {
                dataDict[(int)abilityType] = 1;
            }

            return level * _abilityData.UpgradePrice + _abilityData.FirstUpgradePrice;
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

            if (_unlockableObjectList.Count > UnlockableObjectCount)
            {
                UnlockableObject nextUnlockableObject = _unlockableObjectList[UnlockableObjectCount];
                _unlockableBuyer.Initialize(nextUnlockableObject, PaidAmount, nextUnlockableObject.GetBuyPointPosition,
                    nextUnlockableObject.GetBuyPointRotation);
            }
        }
    }
}





























