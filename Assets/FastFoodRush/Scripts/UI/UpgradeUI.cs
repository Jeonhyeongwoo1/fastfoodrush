using System;
using System.Collections.Generic;
using FastFoodRush.Manager;
using TMPro;
using UnityEngine;
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
            public Button adButton;
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
            _upgradeElementList.ForEach(v => v.upgradeButton.onClick.AddListener(() => OnClickUpgrade(v.abilityType, false)));
            _upgradeElementList.ForEach(v => v.adButton.onClick.AddListener(() => OnClickAdButton(v.abilityType)));
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void OnClickAdButton(AbilityType abilityType)
        {
            AdManager.Instance.ShowRewardAd(() =>
            {
                OnClickUpgrade(abilityType, true);
            }, () =>
            {
                //fail popup
                
            });
        }

        private void OnClickUpgrade(AbilityType abilityType, bool isFree)
        {
            RestaurantManager.Instance.onAbilityUpgradeAction?.Invoke(abilityType, isFree);
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
                element.priceText.text = price == 0 ? "Free" : price.ToString();

                List<Image> guageList = element.guageList;
                int count = guageList.Count;
                for (int i = 0; i < count; i++)
                {
                    guageList[i].color = i < level ? _upgradeColor : _defaultColor;
                }

                bool isMaxLevel = level == guageList.Count;
                bool isPossiblePurchase = price <= manager.Money;
                element.upgradeButton.interactable = isPossiblePurchase && !isMaxLevel;
                element.adButton.interactable = !isMaxLevel;
            }
        }
    }
}








