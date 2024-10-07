using System;
using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FastFoodRush.UI
{
    public class MapUI : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _mapObjList;
        [SerializeField] private List<GameObject> _restaurantObjList;
        [SerializeField] private Button _closeButton;

        private void Start()
        {
            _closeButton.onClick.AddListener(Close);
        }

        private void OnEnable()
        {
            List<bool> restaurantUnlockList = new List<bool>(_restaurantObjList.Count);
            for (int i = 0; i < _restaurantObjList.Count; i++)
            {
                GameObject obj = _restaurantObjList[i];
                var data = SaveSystem.LoadRestaurantData(obj.name);
                restaurantUnlockList.Add(data?.isUnlock ?? false);
            }
            
            OpenMapUI(restaurantUnlockList);
        }

        private void OpenMapUI(List<bool> restaurantUnlockList)
        {
            for (int i = 0; i < _mapObjList.Count; i++)
            {
                GameObject obj = _mapObjList[i];
                obj.SetActive(restaurantUnlockList[i]);
            }
        }

        public void OnClickMapButton(string restaurantId)
        {
            RestaurantManager.Instance.LoadOtherStage(restaurantId);
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }
    }
}