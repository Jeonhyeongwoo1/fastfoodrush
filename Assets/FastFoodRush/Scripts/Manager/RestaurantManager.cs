using System;
using System.Collections.Generic;
using DG.Tweening;
using FastFoodRush.Controller;
using FastFoodRush.Interactable;
using FastFoodRush.Object;
using FastFoodRush.Scripts.Data;
using FastFoodRush.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

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
        Trash,
        Money
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

        public int Money
        {
            get => _data.money;
            set
            {
                _data.money = value;
                onUpdateMoneyAction?.Invoke(_data.money);
            }
        }

        public int PaidAmount
        {
            get => _data.paidAmount;
            set => _data.paidAmount = value;
        }
        
        public Action<float> onUpdateProgressAction;
        public Action<int> onUpdateMoneyAction;
        public Action<AbilityType> onAbilityUpgradeAction;
        public Action<AbilityType, float> onUpgradedAbilityAction;

        public List<ObjectStack> ObjectStacks { get; set; } = new();
        public List<Pile> Piles { get; set; } = new();
        public TrashBin TrashBin { get; set; }
        public UnlockableBuyer UnlockableBuyer => _unlockableBuyer;
        
        [SerializeField] private UnlockableBuyer _unlockableBuyer;
        [SerializeField] private List<UnlockableObject> _unlockableObjectList;
        [SerializeField] private AbilityConfigData _abilityData;
        [SerializeField] private Transform _employeeSpawnPoint;
        [SerializeField] private GameObject _unlockEffectObj; 
        
        [SerializeField] private int _priceOfFood;
        
        private List<EmployeeController> _employeeControllerList = new();
        private RestaurantData _data;

        private void Awake()
        {
            _data = new RestaurantData();
        }

        private void Start()
        {
            string id = SceneManager.GetActiveScene().name;
            SaveSystem.SaveLastPlayRestaurantId($"{id}");
            _data.isUnlock = true;
            LoadRestaurantData(id);
            CheckProgress();
            AudioManager.Instance.PlayBGM(AudioKey.BGM, 0.5f);
            
            onAbilityUpgradeAction += OnAbilityUpgrade;
        }

        private void OnApplicationQuit()
        {
            string id = SceneManager.GetActiveScene().name;
            SaveSystem.SaveRestaurantData(id, _data);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                string id = SceneManager.GetActiveScene().name;
                SaveSystem.SaveRestaurantData(id, _data);
            }
        }

        private void LoadRestaurantData(string id)
        {
            AllDisableUnlockableObject();
            
            RestaurantData restaurantData = SaveSystem.LoadRestaurantData(id);
            if (restaurantData != null)
            {
                _data = restaurantData;
                onUpdateMoneyAction?.Invoke(_data.money);

                int unlockCount = _data.unlockableObjectCount;
                for (int i = 0; i < unlockCount; i++)
                {
                    UnlockObject(i);
                }

                InitializeUnlockableBuyer(unlockCount);

                try
                {
                    int employeeCount = _data.abilityLevelDataDict[(int)AbilityType.EmployeeAmount].level;
                    for (int i = 0; i < employeeCount; i++)
                    {
                        SpawnEmployee();
                    }
                }
                catch (Exception e){}
                
                return;
            }
            
            Money = Const.StartMoney;
            InitializeUnlockableBuyer(0);
            // UnlockObject(0);
        }

        [SerializeField] private int _unlockIndex;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Unlock_Debug();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                OnApplicationPause(true);
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
            onAbilityUpgradeAction -= OnAbilityUpgrade;
        }

        private void OnAbilityUpgrade(AbilityType abilityType)
        {
            int currentMoney = Money;
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
            Money -= price;

            switch (abilityType)
            {
                case AbilityType.EmployeeAmount:
                    SpawnEmployee();
                    break;
            }
            
            AudioManager.Instance.PlaySFX(AudioKey.Kaching);
            onUpgradedAbilityAction?.Invoke(abilityType, abilityData.statusValue);
        }

        private void SpawnEmployee()
        {
            GameObject employeeObj = PoolManager.Instance.Get(PoolKey.Employee);

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
            UnlockableObject unlockableObject = UnlockObject(UnlockableObjectCount);
            if (unlockableObject == null)
            {
                return;
            }
            
            UnlockableObjectCount++;
            PaidAmount = 0;
            AudioManager.Instance.PlaySFX(AudioKey.Magical);

            ShowUnlockEffect(unlockableObject.transform.position);
            if (_unlockableObjectList.Count > UnlockableObjectCount)
            {
                InitializeUnlockableBuyer(UnlockableObjectCount);
            }
            else
            {
                EndStage();
            }
            
            CheckProgress();
        }

        private void EndStage()
        {
            _unlockableBuyer.gameObject.SetActive(false);
            TutorialManager.Instance.CheckMainTutorialCompletion(MainTutorialType.None);
        }

        private void InitializeUnlockableBuyer(int index)
        {
            UnlockableObject target = _unlockableObjectList[index];
            _unlockableBuyer.Initialize(target, PaidAmount, target.GetBuyPointPosition, target.GetBuyPointRotation);
            target.MainTutorialProgress();
        }

        private UnlockableObject UnlockObject(int index)
        {
            if (_unlockableObjectList.Count <= UnlockableObjectCount)
            {
                Debug.LogWarning($"already opened all unlockable object");
                return null;
            }
            
            UnlockableObject unlockableObject = _unlockableObjectList[index];
            unlockableObject.Unlock();

            return unlockableObject;
        }

        private void ShowUnlockEffect(Vector3 spawnPosition)
        {
            _unlockEffectObj.transform.position = spawnPosition;
            _unlockEffectObj.SetActive(true);
            DOVirtual.DelayedCall(1, () => _unlockEffectObj.SetActive(false));
        }

        private void CheckProgress()
        {
            float ratio = (float) UnlockableObjectCount / _unlockableObjectList.Count;
            onUpdateProgressAction?.Invoke(ratio);
        }

        public void LoadOtherStage()
        {
            Scene scene = SceneManager.GetActiveScene();
            int index = scene.buildIndex;
            SceneManager.LoadScene(++index);
        }

        public Vector3 GetOffsetByStackType(StackType stackType)
        {
            switch (stackType)
            {
                case StackType.Food:
                    return new Vector3(0.25f, 0.25f, 0);
                case StackType.Package:
                    return new Vector3(0.25f, 0.25f, 0);
                case StackType.Trash:
                    return new Vector3(0.25f, 0.25f, 0);
                default:
                    return new Vector3(0f, 0.25f, 0);
            }
        }

        public int GetTipAmount()
        {
            int random = Random.Range(1, 3);
            float ratio = random * 0.1f;
            int tipAmount = (int)(_priceOfFood * ratio) + (int)GetStatusValue(AbilityType.PlayerProfit);
            return tipAmount;
        }

        public int GetPriceOfFood(OrderInfoType orderInfoType)
        {
            int price = _priceOfFood * (orderInfoType == OrderInfoType.Package ? 4 : 1) + (int)GetStatusValue(AbilityType.PlayerProfit);
            return price;
        }
    }
}





























