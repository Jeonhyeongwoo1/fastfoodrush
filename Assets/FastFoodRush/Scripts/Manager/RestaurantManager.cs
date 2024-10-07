using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FastFoodRush.Controller;
using FastFoodRush.Interactable;
using FastFoodRush.Object;
using FastFoodRush.Scripts.Data;
using FastFoodRush.UI;
using Sirenix.OdinInspector;
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
        public Action<AbilityType, bool> onAbilityUpgradeAction;
        public Action<AbilityType, float> onUpgradedAbilityAction;

        public List<ObjectStack> ObjectStacks { get; set; } = new();
        public List<Pile> Piles { get; set; } = new();
        public TrashBin TrashBin { get; set; }
        public List<BurgerMachine> FoodMachineList { get; set; } = new();
        public List<BaseCounterTable> BaseCounterTableList { get; set; } = new();
        public UnlockableBuyer UnlockableBuyer => _unlockableBuyer;
        
        [SerializeField] private UnlockableBuyer _unlockableBuyer;
        [SerializeField] private List<UnlockableObject> _unlockableObjectList;
        [SerializeField] private AbilityConfigData _abilityData;
        [SerializeField] private RestaurantConfigData _restaurantConfigData;
        [SerializeField] private Transform _employeeSpawnPoint;
        [SerializeField] private GameObject _unlockEffectObj;
        [SerializeField] private GameObject _stageClearEffectObj;
        [SerializeField] private GameObject _stageClearText;
        [SerializeField] private MapUI _mapUI;
        
        private List<EmployeeController> _employeeControllerList = new();
        private RestaurantData _data;

        private void Awake()
        {
            string id = SceneManager.GetActiveScene().name;
            SaveSystem.SaveLastPlayRestaurantId($"{id}");;
            LoadRestaurantData(id);
            
            _data.isUnlock = true;
            SaveRestaurantData();
            CheckProgress();
        }

        private void Start()
        {
            AudioManager.Instance.PlayBGM(AudioKey.BGM, 0.5f);
            onAbilityUpgradeAction += OnAbilityUpgrade;
        }

        private void OnApplicationQuit()
        {
            SaveRestaurantData();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                SaveRestaurantData();
            }
        }

        private void SaveRestaurantData()
        {
            string id = SceneManager.GetActiveScene().name;
            SaveSystem.SaveRestaurantData(id, _data);
        }
        
        private void LoadRestaurantData(string id)
        {
            AllDisableUnlockableObject();
            
            RestaurantData restaurantData = SaveSystem.LoadRestaurantData(id);
            Debug.Log($"res " +restaurantData);
            if (restaurantData != null)
            {
                _data = restaurantData;
                onUpdateMoneyAction?.Invoke(_data.money);

                int unlockCount = _data.unlockableObjectCount;
                for (int i = 0; i < unlockCount; i++)
                {
                    UnlockObject(i);
                }

                if (unlockCount < _unlockableObjectList.Count)
                {
                    InitializeUnlockableBuyer(unlockCount);
                }

                try
                {
                    int employeeCount = _data.abilityLevelDataDict[(int)AbilityType.EmployeeAmount].level;
                    for (int i = 0; i < employeeCount; i++)
                    {
                        SpawnEmployee();
                    }
                }
                catch (Exception e)
                {
                    Debug.Log($"failed create employee " + e);
                    CreateAbilityData(AbilityType.EmployeeAmount);
                }
                
                return;
            }
            
            _data = new RestaurantData();
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

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Money += 1000;
            }
        }

        private void Unlock_Debug()
        {
            BuyUnlockableObject();
        }

        private void OnDestroy()
        {
            onAbilityUpgradeAction -= OnAbilityUpgrade;
        }

        private void OnAbilityUpgrade(AbilityType abilityType, bool isFree)
        {
            int price = GetCurrentAbilityPrice(abilityType);
            if (!isFree)
            {
                int currentMoney = Money;
                if (currentMoney < price)
                {
                    Debug.LogWarning($"failed to upgrade ability current moneny {currentMoney} / price {price}");
                    return;
                }
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
            
            if (!isFree)
            {
                Money -= price;
            }
            
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

            switch (abilityType)
            {
                case AbilityType.PlayerSpeed:
                case AbilityType.PlayerProfit:
                case AbilityType.EmployeeSpeed:
                case AbilityType.EmployeeCapacity:
                    return abilityData.level * _abilityData.UpgradePrice + _abilityData.FirstUpgradePrice;
                case AbilityType.PlayerCapacity:
                case AbilityType.EmployeeAmount:
                    if (abilityData.level == 0)
                    {
                        return 0;
                    }

                    return abilityData.level * _abilityData.UpgradePrice + _abilityData.FirstUpgradePrice;
                default:
                    Debug.LogError($"failed ability type price {abilityType}");
                    return 0;
             }
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
            
            unlockableObject.CompleteMainTutorialProgress();
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
            _stageClearText.gameObject.SetActive(true);
            _stageClearEffectObj.SetActive(true);
            _unlockableBuyer.gameObject.SetActive(false);

            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                RestaurantData restaurantData = new RestaurantData();
                restaurantData.isUnlock = true;
                string scenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                SaveSystem.SaveRestaurantData(sceneName, restaurantData);
                DOVirtual.DelayedCall(3, () =>
                {
                    _mapUI.gameObject.SetActive(true);
                });
            }
            else
            {
                Debug.Log("Last restaurant");
            }
        }

        private void InitializeUnlockableBuyer(int index)
        {
            _unlockableBuyer.gameObject.SetActive(false);
            StartCoroutine(VerifyRunningTutorialCor(() =>
            {
                UnlockableObject target = _unlockableObjectList[index];
                _unlockableBuyer.Initialize(target, GetMoneyNeedToUnlock(), PaidAmount, target.GetBuyPointPosition,
                    target.GetBuyPointRotation);
                _unlockableBuyer.gameObject.SetActive(true);
                target.LoadMainTutorial();
            }));
        }

        private IEnumerator VerifyRunningTutorialCor(Action done)
        {
            yield return new WaitForEndOfFrame();
            while (TutorialManager.Instance.IsRunningTutorial())
            {
                yield return null;
            }
            
            done?.Invoke();
        }

        private int GetMoneyNeedToUnlock()
        {
            return _restaurantConfigData.MoneyNeedToUnlock * (UnlockableObjectCount + 1);
        }

        private UnlockableObject UnlockObject(int index)
        {
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

        public void LoadOtherStage(string resturantId)
        {
            SceneManager.LoadScene(resturantId);
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
            // int random = Random.Range(1, 3);
            // float ratio = random * 0.1f;
            // int tipAmount = (int)(_restaurantConfigData.PriceOfFood * ratio) + (int)GetStatusValue(AbilityType.PlayerProfit);

            int tipAmount = (int)(Random.Range(1, _restaurantConfigData.PriceOfFood * 0.5f)) +
                            (int)GetStatusValue(AbilityType.PlayerProfit);
            return tipAmount;
        }

        public int GetPriceOfFood(OrderInfoType orderInfoType)
        {
            int price = _restaurantConfigData.PriceOfFood * (orderInfoType == OrderInfoType.Package ? 4 : 1) +
                        (int)GetStatusValue(AbilityType.PlayerProfit);
            return price;
        }

        [Button]
        private void DeleteAllPlayerPrefabsData()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}





























