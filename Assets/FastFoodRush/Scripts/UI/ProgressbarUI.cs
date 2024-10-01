using System;
using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Manager;
using TMPro;
using UnityEngine;

namespace FastFoodRush.UI
{
    public class ProgressbarUI : MonoBehaviour
    {
        [SerializeField] private SlicedFilledImage _progressbarImage;
        [SerializeField] private TextMeshProUGUI _progressbarText;

        private void OnEnable()
        {
            RestaurantManager.Instance.onUpdateProgressAction += UpdateProgress;
        }

        private void OnDisable()
        {
            if (RestaurantManager.Instance == null)
            {
                RestaurantManager.Instance.onUpdateProgressAction -= UpdateProgress;
            }
        }

        private void UpdateProgress(float ratio)
        {
            _progressbarImage.fillAmount = ratio;
            _progressbarText.text = string.Format("PROGRESS {0:0.00}%", ratio * 100);
        }
    }
}