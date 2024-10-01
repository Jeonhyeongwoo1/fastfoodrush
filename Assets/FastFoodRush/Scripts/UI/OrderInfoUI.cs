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
        [SerializeField] private GameObject iconObj;
        
        private void Start()
        {
            gameObject.SetActive(false);
        }
        
        public void Show(string amount, int orderInfoType, Vector3 position)
        {
            if (orderInfoType != (int)_orderInfoType)
            {
                return;
            }

            iconObj.SetActive(amount != Const.NoSeat);
            _amountText.text = amount;
            transform.position = position;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
