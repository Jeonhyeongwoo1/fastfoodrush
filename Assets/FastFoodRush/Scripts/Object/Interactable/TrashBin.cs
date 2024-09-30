using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FastFoodRush.Manager;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class TrashBin : Interactable
    {
        private float _elapsed;

        private void Start()
        {
            RestaurantManager.Instance.TrashBin = this;
        }

        private void Update()
        {
            if (_player == null || _player.Stack.StackCount == 0)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            if (_elapsed > 0.1f)
            {
                _elapsed = 0;
                GameObject obj = _player.Stack.Pop();
                RemoveTrash(obj);
            }
        }

        public void RemoveTrash(GameObject trashObj)
        {
            Vector3 endValue = transform.position;
            trashObj.transform.DOJump(endValue, 2, 1, 0.25f).OnComplete(() =>
            {
                trashObj.gameObject.SetActive(false);
            });   
        }
    }
}