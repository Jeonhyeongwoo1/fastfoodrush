using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class TrashBin : Interactable
    {
        private float _elapsed = 0;
        private float _interval = 0.1f;
        
        private void Update()
        {
            if (_player == null || _player.Stack.StackCount == 0)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            if (_elapsed > _interval)
            {
                _elapsed = 0;
                GameObject obj = _player.Stack.Pop();
                Vector3 endValue = transform.position;
                obj.transform.DOJump(endValue, 2, 1, 0.25f).OnComplete(() =>
                {
                    obj.gameObject.SetActive(false);
                });
            }
        }
    }
}