using System;
using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Manager;
using TMPro;
using UnityEngine;

namespace FastFoodRush.UI
{
    public enum OrderInfoType
    {
        Food,
        Package
    }
    
    public class OrderInfoUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private OrderInfoType _orderInfoType;

        private void Start()
        {
            RestaurantManager.Instance.onOrderProduct += Show;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            RestaurantManager.Instance.onOrderProduct -= Show;
        }

        public void Show(int amount, int orderInfoType, Vector3 position)
        {
            if (orderInfoType != (int)_orderInfoType)
            {
                return;
            }

            _amountText.text = amount.ToString();
            transform.position = position;
            gameObject.SetActive(amount != 0);
        }

        public void Hide()
        {
            
        }
    }
}
