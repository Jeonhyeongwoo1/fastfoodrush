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
        public Action<AbilityType, float> onUpgradedAbility;

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

            Dictionary<int, AbilityData> dataDict = _data.abilityLevelDataDict;
            if (!dataDict.TryGetValue((int)abilityType, out AbilityData abilityData))
            {
                abilityData = CreateAbilityData(abilityType);
            }

            int level = abilityData.level;
            if (_abilityData.MaxLevel == level)
            {
                Debug.LogWarning($"ability is max level : {level}");
                return;
            }

            abilityData.statusValue = _abilityData.GetPlayerStatusValue(abilityType, ++abilityData.level);
            dataDict[(int)abilityType] = abilityData;
            //UseMoney
            Moneny -= price;
            onUpgradedAbility?.Invoke(abilityType, abilityData.statusValue);
        }

        public int GetCurrentAbilityLevel(AbilityType abilityType)
        {
            Dictionary<int, AbilityData> dataDict = _data.abilityLevelDataDict;
            if (dataDict.TryGetValue((int)abilityType, out AbilityData abilityData))
            {
                return abilityData.level;
            }

            abilityData = CreateAbilityData(abilityType);
            dataDict[(int)abilityType] = abilityData;

            return 0;
        }

        private AbilityData CreateAbilityData(AbilityType abilityType)
        {
            AbilityData abilityData = new AbilityData();
            abilityData.level = 0;
            abilityData.statusValue = _abilityData.GetDefaultAbilityValue(abilityType);
            _data.abilityLevelDataDict[(int)abilityType] = abilityData;
            return abilityData;
        }

        public float GetDefaultAbilityValue(AbilityType abilityType) =>
            _abilityData.GetDefaultAbilityValue(abilityType);

        public int GetCurrentAbilityPrice(AbilityType abilityType)
        {
            Dictionary<int, AbilityData> dataDict = _data.abilityLevelDataDict;
            if (!dataDict.TryGetValue((int)abilityType, out AbilityData abilityData))
            {
                abilityData = CreateAbilityData(abilityType);
            }

            return abilityData.level * _abilityData.UpgradePrice + _abilityData.FirstUpgradePrice;
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





























