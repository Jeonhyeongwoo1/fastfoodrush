using FastFoodRush.Manager;
using FastFoodRush.Object;
using FastFoodRush.Scripts.Data;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class BurgerMachine : UnlockableObject
    {
        [SerializeField] private BurgerMachineConfigData _burgerMachineConfigData;
        
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
        }

        protected override void UpgradeableMesh()
        {
            base.UpgradeableMesh();

            if (_unlockLevel >= Const.MaxLevel)
            {
                foreach (FlippingObject flippingObject in _flippingObjectArray)
                {
                    flippingObject.gameObject.SetActive(false);
                }
                
                _movingObject.Moving(_burgerMachineConfigData.CreateTime);
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
            _createTime = _burgerMachineConfigData.CreateTime * (1 - 0.2f * (_unlockLevel - 1));
            _capacity = _burgerMachineConfigData.Capacity + (_unlockLevel - 1) * 2;
        }

        public override void MainTutorialProgress()
        {
            TutorialManager tutorialManager = TutorialManager.Instance;
            bool isExecutedTutorial =  tutorialManager.CheckExecutedMainTutorialProgress(MainTutorialType.FoodMachine);
            if (!isExecutedTutorial)
            {
                tutorialManager.SetTutorialTarget(RestaurantManager.Instance.UnlockableBuyer.transform);
                tutorialManager.LoadTutorial(MainTutorialType.FoodMachine);
            }
        }
    }
}