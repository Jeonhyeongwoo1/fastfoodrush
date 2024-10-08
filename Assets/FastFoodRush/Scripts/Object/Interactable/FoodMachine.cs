using FastFoodRush.Manager;
using FastFoodRush.Object;
using FastFoodRush.Scripts.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace FastFoodRush.Interactable
{
    public class FoodMachine : UnlockableObject
    {
        public Transform FoodPileTransform => _foodPile.transform;
        
        [FormerlySerializedAs("_burgerMachineConfigData")] [SerializeField] private FoodMachineConfigData foodMachineConfigData;
        
        private FoodPile _foodPile;
        private float elapsed = 0;
        private FlippingObject[] _flippingObjectArray;
        private MovingObject _movingObject;

        private int _capacity;
        private float _createTime;

        protected override void Start()
        {
            base.Start();
            _foodPile = GetComponentInChildren<FoodPile>(true);
            _flippingObjectArray = GetComponentsInChildren<FlippingObject>(true);
            _movingObject = GetComponentInChildren<MovingObject>(true);
        }

        public override void Unlock(bool animate = true)
        {
            base.Unlock(animate);
            
            SetData();
            RestaurantManager.Instance.FoodMachineList.Add(this);
        }

        protected override void UpgradeableMesh()
        {
            base.UpgradeableMesh();

            if (_unlockLevel >= Const.MaxLevel)
            {
                if (_flippingObjectArray == null)
                {
                    _flippingObjectArray = GetComponentsInChildren<FlippingObject>(true);
                }

                if (_flippingObjectArray != null)
                {
                    foreach (FlippingObject flippingObject in _flippingObjectArray)
                    {
                        flippingObject.gameObject.SetActive(false);
                    }
                }

                if (_movingObject == null)
                {
                    _movingObject = GetComponentInChildren<MovingObject>(true);
                }

                if (_movingObject != null)
                {
                    _movingObject.Moving(foodMachineConfigData.DefaultCreateTime);
                }
            }
        }

        private void Update()
        {
            if (_capacity <= _foodPile.StackCount)
            {
                return;
            }

            elapsed += Time.deltaTime;
            if (elapsed > _createTime)
            {
                _foodPile.Drop();
                elapsed = 0;
            }
        }
        
        private void SetData()
        {
            _createTime = foodMachineConfigData.DefaultCreateTime * (1 - 0.2f * (_unlockLevel - 1));
            _capacity = foodMachineConfigData.DefaultCapacity + (_unlockLevel - 1) * 2;
        }

        public override void LoadMainTutorial()
        {
            TutorialManager tutorialManager = TutorialManager.Instance;
            bool isExecutedTutorial =  tutorialManager.CheckExecutedMainTutorialProgress(MainTutorialType.FoodMachine);
            if (!isExecutedTutorial)
            {
                tutorialManager.SetTutorialTarget(RestaurantManager.Instance.UnlockableBuyer.transform);
                tutorialManager.LoadTutorial(MainTutorialType.FoodMachine);
            }
        }

        public override void CompleteMainTutorialProgress()
        {
            TutorialManager.Instance.CompleteMainTutorialDepth(MainTutorialType.FoodMachine);
        }
    }
}