using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Manager;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public abstract class Pile : Interactable
    {
        public int StackCount => _objectStack.Count;
        public bool IsExistObject => _objectStack.Count > 0;
        public StackType StackType => _stackType;

        [SerializeField] protected StackType _stackType;
        
        protected virtual float _timeInterval { get; private set; }
        protected Stack<GameObject> _objectStack = new();
        private float _elapsed = 0;
        
        protected virtual void Update()
        {
            if (_player == null || !IsExistObject || _player.Stack.CurrentStackType != _stackType && _player.Stack.CurrentStackType != StackType.None)
            {
                return;
            }
            
            _elapsed += Time.deltaTime;
            if (_elapsed > _timeInterval)
            {
                _elapsed = 0;
                if (_player.Stack.Height < _player.PlayerCapacity)
                {
                    _player.Stack.Stack(_objectStack.Pop(), _stackType);
                    AudioManager.Instance.PlaySFX(AudioKey.Pop);
                }
            }
        }

        protected virtual void Start(){}
        public abstract void Drop(GameObject obj = null);
        public GameObject RemoveStack()
        {
            if (!IsExistObject)
            {
                Debug.LogWarning($"isn't exist trash");
                return null;
            }

            GameObject obj = _objectStack.Pop();
            return obj;
        }
    }
}