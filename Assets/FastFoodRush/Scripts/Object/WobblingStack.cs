using System;
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
        public int Height => _height;
        public StackType CurrentStackType => _currentStackType;

        [SerializeField] private GameObject _tray;
        [SerializeField] private StackType _currentStackType;
        
        private List<Transform> _stackList = new List<Transform>();
        private int _height;
        [SerializeField] private Vector2 rateRange = new Vector2(0.8f, 0.4f);
        private Vector2 movement;
        private Vector3 _stackOffset;

        private void Start()
        {
            _stackOffset = RestaurantManager.Instance.GetOffsetByStackType(StackType.Food);
            _stackOffset = new Vector3(0, 0.1f, 0);
        }

        private void Update()
        {
            if (_stackList.Count == 0) return;

            movement.x = SimpleInput.GetAxis("Horizontal");
            movement.y = SimpleInput.GetAxis("Vertical");

            _stackList[0].transform.position = transform.position;
            _stackList[0].transform.rotation = transform.rotation;

            for (int i = 1; i < _stackList.Count; i++)
            {
                float rate = Mathf.Lerp(rateRange.x, rateRange.y, i / (float)_stackList.Count);
                _stackList[i].position = Vector3.Lerp(_stackList[i].position,
                    _stackList[i - 1].position + (_stackList[i - 1].up * _stackOffset.y), rate);

                _stackList[i].rotation = Quaternion.Lerp(_stackList[i].rotation, _stackList[i - 1].rotation, rate);
                if (movement != Vector2.zero) _stackList[i].rotation *= Quaternion.Euler(-i * 0.1f * rate, 0, 0);
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

            Vector3 endValue = transform.position + new Vector3(0, _stackOffset.y, 0) * _stackList.Count;
            _currentStackType = stackType;

            switch (_currentStackType)
            {
                case StackType.Food:
                case StackType.Package:
                    _height++;
                    break;
            }

            obj.transform.DOJump(endValue, 2, 1, 0.25f).OnComplete(() => _stackList.Add(obj.transform));
        }

        public Transform Peek() => _stackList.LastOrDefault();

        public GameObject Pop()
        {
            Transform tr = Peek();
            _stackList.Remove(tr);

            switch (_currentStackType)
            {
                case StackType.Food:
                case StackType.Package:
                    _height--;
                    break;
            }
            
            if (_stackList.Count == 0)
            {
                _tray.SetActive(false);
                _currentStackType = StackType.None;
            }

            return tr.gameObject;
        }
    }
}