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
                var upgradeList = guageList.GetRange(0, level - 1);
                upgradeList.ForEach(v=> v.color = _upgradeColor);

                var notUpgradedList = guageList.GetRange(level - 1, guageList.Count - 1);
                notUpgradedList.ForEach(v=> v.color = _defaultColor);

                bool isPossiblePurchase = price <= manager.Moneny;
                element.upgradeButton.interactable = isPossiblePurchase;
                
            }
        }

    }
}
