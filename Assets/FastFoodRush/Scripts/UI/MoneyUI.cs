using System;
using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Manager;
using TMPro;
using UnityEngine;


namespace FastFoodRush.UI
{
    public class MoneyUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _moneyText;

        private void OnEnable()
        {
            RestaurantManager.Instance.onUseMoneny += OnUseMoney;
        }

        private void OnDisable()
        {
            RestaurantManager.Instance.onUseMoneny -= OnUseMoney;
        }

        private void OnUseMoney(int money)
        {
            _moneyText.text = money.ToString();
        }
    } 
}