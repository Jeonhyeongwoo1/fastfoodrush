using System;
using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace FastFoodRush.UI
{
    public class UpgradeUI : MonoBehaviour
    {
        [Serializable]
        public struct UpgradeElement
        {
            public List<Image> guageList;
            public Button upgradeButton;
            public TextMeshProUGUI priceText;
            public AbilityType abilityType;
        }

        [SerializeField] private Button _closeButton;
        [SerializeField] private List<UpgradeElement> _upgradeElementList;
        [SerializeField] private Color _upgradeColor;
        [SerializeField] private Color _defaultColor;
        
        private void Start()
        {
            _closeButton.onClick.RemoveAllListeners();
            _closeButton.onClick.AddListener(()=> gameObject.SetActive(false));
            _upgradeElementList.ForEach(v => v.upgradeButton.onClick.AddListener(() => OnClickUpgrade(v.abilityType)));
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void OnClickUpgrade(AbilityType abilityType)
        {
            RestaurantManager.Instance.onUpgrade?.Invoke(abilityType);
            Refresh();
        }

        private void Refresh()
        {
            RestaurantManager manager = RestaurantManager.Instance;
            foreach (UpgradeElement element in _upgradeElementList)
            {
                AbilityType abilityType = element.abilityType;
                int level = manager.GetCurrentAbilityLevel(abilityType);
                int price = manager.GetCurrentAbilityPrice(abilityType);
                element.priceText.text = price.ToString();

                List<Image> guageList = element.guageList;
                int count = guageList.Count;
                for (int i = 0; i < count; i++)
                {
                    guageList[i].color = i < level ? _upgradeColor : _defaultColor;
                }
                
                bool isPossiblePurchase = price <= manager.Moneny;
                element.upgradeButton.interactable = isPossiblePurchase;
                
            }
        }

    }
}








