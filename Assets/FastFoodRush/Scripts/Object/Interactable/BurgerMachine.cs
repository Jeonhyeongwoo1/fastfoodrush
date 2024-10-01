using System;
using System.Collections;
using System.Collections.Generic;
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

        private void Start()
        {
            _foodPile = GetComponentInChildren<FoodPile>(true);
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