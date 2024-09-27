using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class ObjectStack : Interactable
    {
        [SerializeField] private Vector3 _offset = new Vector3(0, 0.25f, 0);
        [SerializeField] private int _maxCapacity = 6;
        [SerializeField] private float _interval;

        private List<GameObject> _stackList = new List<GameObject>();
        private float _elapsed = 0;

        private void Update()
        {
            if (_stackList.Count >= _maxCapacity || _player == null)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            if (_elapsed > _interval)
            {
                _elapsed = 0;
                GameObject obj = _player.Stack.Pop();
                Vector3 endValue = transform.position + _offset * _stackList.Count;
                _stackList.Add(obj);
                obj.transform.DOJump(endValue, 2, 1, 0.25f).OnComplete(() =>
                {
                    obj.transform.position = endValue;
                });
            }
        }
    } 
}