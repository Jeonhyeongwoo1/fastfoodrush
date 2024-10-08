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
        public int StackCount => _objectStack.Count;
        public StackType StackType => _stackType;
        public Vector3 ObjectStackPointPosition => _objectStackPoint != null ? _objectStackPoint.position : Vector3.zero;

        [SerializeField] private StackType _stackType;
        [SerializeField] private bool _useMaxCapacity;
        [SerializeField] private int _maxCapacity = 6;
        [SerializeField] private float _interval;
        [SerializeField] private Transform _objectStackPoint;
        [SerializeField] private bool _useCustomOffset;
        [SerializeField] private Vector3 _customOffset;

        private Stack<GameObject> _objectStack = new();
        private float _elapsed = 0;
        private Vector3 _offset;

        private void OnEnable()
        {
            RestaurantManager.Instance.ObjectStacks.Add(this);
            _offset = _useCustomOffset ? _customOffset : RestaurantManager.Instance.GetOffsetByStackType(_stackType);
        }

        private void OnDisable()
        {
            if (RestaurantManager.Instance != null)
            {
                RestaurantManager.Instance.ObjectStacks.Remove(this);
            }
        }

        private void Update()
        {
            if (_player == null || _player.Stack.StackCount == 0 || _player.Stack.CurrentStackType != _stackType)
            {
                return;
            }

            if (_useMaxCapacity)
            {
                if (_objectStack.Count >= _maxCapacity)
                {
                    return;
                }
            }

            _elapsed += Time.deltaTime;
            if (_elapsed > _interval)
            {
                _elapsed = 0;
                GameObject obj = _player.Stack.Pop();
                PlaySound();
                DoStackAnimation(obj);

                if (_player.Stack.StackCount == 0)
                {
                    TutorialManager.Instance.CompleteMainTutorialDepth(MainTutorialType.FirstSeat);
                }
            }
        }

        private void PlaySound()
        {
            switch (_stackType)
            {
                case StackType.Food:
                case StackType.Package:
                    AudioManager.Instance.PlaySFX(AudioKey.Pop);
                    break;
                case StackType.Trash:
                    AudioManager.Instance.PlaySFX(AudioKey.Trash);
                    break;
            }
        }

        private void DoStackAnimation(GameObject obj)
        {
            Vector3 endValue = transform.position +
                               new Vector3(0, _offset.y, 0) *
                               _objectStack.Count;
            _objectStack.Push(obj);
            obj.transform.DOJump(endValue, 2, 1, _interval);
            obj.transform.eulerAngles = Vector3.zero;
        }

        public void Stack(GameObject obj)
        {
            DoStackAnimation(obj);
        }

        public GameObject GetStackObject()
        {
            if (_objectStack.Count == 0)
            {
                Debug.LogWarning($"object count is 0");
                return null;
            }
            DOTween.Kill(true);            
            return _objectStack.Pop();
        }
    } 
}