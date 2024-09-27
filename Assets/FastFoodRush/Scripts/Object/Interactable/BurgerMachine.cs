using System;
using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Object;
using FastFoodRush.Scripts.Data;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public enum ProductType { Food, }
    
    public class BurgerMachine : UnlockableObject
    {
        [SerializeField] private BurgerMachineConfigData _burgerMachineConfigData;
        [SerializeField] private ProductType _productType;
        
        private FoodPile _foodPile;
        private float elapsed = 0;

        private void Start()
        {
            _foodPile = GetComponentInChildren<FoodPile>(true);
        }

        private void Update()
        {
            BurgerMachineConfigData data = _burgerMachineConfigData;

            if (data.capacity <= _foodPile.DroppedFoodCount)
            {
                return;
            }

            elapsed += Time.deltaTime;
            if (elapsed > data.createTime)
            {
                _foodPile.Drop();
                elapsed = 0;
            }
        }

        public override void Unlock()
        {
            base.Unlock();
        }
    }
}