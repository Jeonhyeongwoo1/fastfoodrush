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

        protected override void Start()
        {
            base.Start();
            _foodPile = GetComponentInChildren<FoodPile>(true);
            _flippingObjectArray = GetComponentsInChildren<FlippingObject>(true);
            _movingObject = GetComponentInChildren<MovingObject>(true);
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
            BurgerMachineConfigData data = _burgerMachineConfigData;

            if (data.Capacity <= _foodPile.StackCount)
            {
                return;
            }

            elapsed += Time.deltaTime;
            if (elapsed > data.CreateTime)
            {
                _foodPile.Drop();
                elapsed = 0;
            }
        }
    }
}