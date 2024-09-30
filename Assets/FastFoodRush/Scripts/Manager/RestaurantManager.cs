using System;
using System.Collections.Generic;
using FastFoodRush.Controller;
using FastFoodRush.Interactable;
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

    public enum StackType
    {
        None,
        Food,
        Package,
        Trash
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

        public List<ObjectStack> ObjectStacks { get; set; } = new();
        public List<Pile> Piles { get; set; } = new();
        public TrashBin TrashBin { get; set; } = new();
        
        [SerializeField] private UnlockableBuyer _unlockableBuyer;
        [SerializeField] private List<UnlockableObject> _unlockableObjectList;
        [SerializeField] private AbilityConfigData _abilityData;
        [SerializeField] private Transform _employeeSpawnPoint;
        
        private List<EmployeeController> _employeeControllerList = new();
        private RestaurantData _data;
        
        private const int startMoneny = 100000;

        private void Awake()
        {
            _data = new RestaurantData();
        }

        private void Start()
        {
            Moneny = startMoneny;
            AllDisableUnlockableObject();
            UnlockableObject unlockableObject = _unlockableObjectList[UnlockableObjectCount];
            _unlockableBuyer.Initialize(unlockableObject, PaidAmount, unlockableObject.GetBuyPointPosition, unlockableObject.GetBuyPointRotation);

            onUpgrade += OnAbilityUpgrade;
        }

        [SerializeField] private int _unlockIndex;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Unlock_Debug();
            }
        }

        private void Unlock_Debug()
        {
            for (int i = 0; i < _unlockIndex; i++)
            {
                BuyUnlockableObject();
            }
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

            abilityData.statusValue = _abilityData.GetStatusValue(abilityType, ++abilityData.level);
            dataDict[(int)abilityType] = abilityData;
            //UseMoney
            Moneny -= price;

            switch (abilityType)
            {
                case AbilityType.EmployeeAmount:
                    SpawnEmployee();
                    break;
            }
            
            onUpgradedAbility?.Invoke(abilityType, abilityData.statusValue);
        }

        private void SpawnEmployee()
        {
            GameObject employeeObj = PoolManager.Instance.Get(Key.Employee);

            if (employeeObj.TryGetComponent(out EmployeeController employeeController))
            {
                _employeeControllerList.Add(employeeController);
            }

            employeeController.Initialize(_employeeSpawnPoint.position, GetStatusValue(AbilityType.EmployeeSpeed),
                (int) GetStatusValue(AbilityType.EmployeeCapacity));
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
        
        public float GetStatusValue(AbilityType abilityType) => _abilityData.GetStatusValue(abilityType, GetCurrentAbilityLevel(abilityType));
        
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

        public Vector3 GetOffsetByStackType(StackType stackType)
        {
            switch (stackType)
            {
                case StackType.Food:
                    return new Vector3(0.25f, 0.25f, 0);
                case StackType.Package:
                    return new Vector3(0, 0.25f, 0);
                case StackType.Trash:
                    return new Vector3(0.25f, 0.25f, 0);
                default:
                    return new Vector3(0f, 0.25f, 0);
            }
        }
    }
}





























