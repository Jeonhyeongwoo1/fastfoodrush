using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FastFoodRush.Manager;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class WobblingStack : MonoBehaviour
    {
        public int StackCount => _stackList.Count;
        public StackType CurrentStackType => _currentStackType;

        [SerializeField] private GameObject _tray;
        [SerializeField] private Vector3 _offset = new Vector3(0, 0.25f, 0);

        [SerializeField] private StackType _currentStackType;
        [SerializeField] private List<GameObject> _stackList = new();

        private void Update()
        {
            if (_stackList.Count == 0)
            {
                _currentStackType = StackType.None;
                return;
            }

            for (int i = 0; i < _stackList.Count; i++)
            {
                GameObject go = _stackList[i];
                go.transform.position = _tray.transform.position + _offset * i;
            }
        }

        public void Stack(GameObject obj, StackType stackType)
        {
            if (_currentStackType != StackType.None && _currentStackType != stackType)
            {
                return;
            }
            
            if (!_tray.activeSelf)
            {
                _tray.SetActive(true);
            }

            Vector3 endValue = _tray.transform.position + _offset * _stackList.Count;
            obj.transform.DOJump(endValue, 2, 1, 0.25f).OnComplete(() =>
            {
                _stackList.Add(obj);
                _currentStackType = stackType;
                obj.transform.position = endValue;
            });
        }

        public GameObject Pop()
        {
            GameObject go = _stackList.LastOrDefault();
            _stackList.Remove(go);
            if (_stackList.Count == 0)
            {
                _tray.SetActive(false);
            }
            
            return go;
        }
    }
}