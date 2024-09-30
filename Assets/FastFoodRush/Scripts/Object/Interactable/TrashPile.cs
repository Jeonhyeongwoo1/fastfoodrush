using System;
using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FastFoodRush.Interactable
{
    public class TrashPile : Interactable
    {
        public bool IsExistTrash => _trashStack.Count > 0;
        
        [SerializeField] private StackType _stackType;
        
        private Stack<GameObject> _trashStack = new();
        private float _elapsed = 0;
        private float _timeInterval = 0.05f;

        private void Update()
        {
            if (_player == null || _trashStack.Count == 0 || _player.Stack.CurrentStackType != _stackType)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            if (_elapsed > _timeInterval)
            {
                _elapsed = 0;
                if (_player.Stack.StackCount < _player.PlayerCapacity)
                {
                    _player.Stack.Stack(_trashStack.Pop(), StackType.Trash);
                }
            }
        }

        public void Stack(int trashCount)
        {
            for (int i = 0; i < trashCount; i++)
            {
                GameObject obj = PoolManager.Instance.Get(Key.Trash);

                Vector3 random = new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
                Vector3 offset = (_trashStack.Count % 2 == 0) ? -Vector3.left * 0.25f + random : Vector3.left * 0.25f + random;
                Vector3 spawnPosition = transform.position + offset;
            
                obj.transform.position = spawnPosition;
                obj.SetActive(true);
                _trashStack.Push(obj);
            }
        }
    }
}