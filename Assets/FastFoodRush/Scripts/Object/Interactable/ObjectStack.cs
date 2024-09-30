using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FastFoodRush.Manager;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class ObjectStack : Interactable
    {
        public int StackCount => _stackList.Count;

        [SerializeField] private StackType _stackType;
        [SerializeField] private Vector3 _offset = new Vector3(0, 0.25f, 0);
        [SerializeField] private bool _useMaxCapacity;
        [SerializeField] private int _maxCapacity = 6;
        [SerializeField] private float _interval;

        [SerializeField] private List<GameObject> _stackList = new List<GameObject>();
        private float _elapsed = 0;

        private void Update()
        {
            if (_player == null || _player.Stack.StackCount == 0 || _player.Stack.CurrentStackType != _stackType)
            {
                return;
            }

            if (_useMaxCapacity)
            {
                if (_stackList.Count >= _maxCapacity)
                {
                    return;
                }
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

        public GameObject GetStackObject()
        {
            if (_stackList.Count == 0)
            {
                Debug.LogWarning($"object count is 0");
                return null;
            }

            GameObject go = _stackList.LastOrDefault();
            _stackList.Remove(go);
            return go;
        }
    } 
}