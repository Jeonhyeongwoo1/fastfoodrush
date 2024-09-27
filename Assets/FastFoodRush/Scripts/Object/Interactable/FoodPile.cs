using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FastFoodRush.Manager;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class FoodPile : Interactable
    {
        public int DroppedFoodCount => _droppedFoodList.Count;
        
        [SerializeField] private Vector3 _offset = new Vector3(0.25f, 0.25f, 0);
        
        private List<GameObject> _droppedFoodList = new ();
        private float _elapsed = 0;
        private float _timeInterval = 0.2f;
        
        private void Update()
        {
            if (_player == null || _droppedFoodList.Count == 0)
            {
                return;
            }

            _elapsed += Time.deltaTime;
            if (_elapsed > _timeInterval)
            {
                _elapsed = 0;
                _player.Stack.Stack(_droppedFoodList.Last());
                _droppedFoodList.RemoveAt(_droppedFoodList.Count-1);
            }
        }
        
        public void Drop()
        {
            GameObject go = PoolManager.Instance.Get(Key.Food);
            
            _droppedFoodList.Add(go);
            int count = _droppedFoodList.Count - 1;
            bool isLeft = count % 2 == 0;
            int depth = count / 2;

            Vector3 position = transform.position + (isLeft
                ? new Vector3(-_offset.x, depth * _offset.y, 0)
                : new Vector3(_offset.x, depth * _offset.y, 0));

            go.transform.position = position;
            go.gameObject.SetActive(true);
        }

        protected override void OnPlayerEnter(Transform tr)
        {
            base.OnPlayerEnter(tr);
            
            
        }
    }
}