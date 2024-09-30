using System;
using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Manager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FastFoodRush.Interactable
{
    public class TrashPile : Pile
    {
        public Vector3 ObjectStackPointPosition => _objectStackPoint.position;
        
        [SerializeField] private Transform _objectStackPoint;

        protected override float _timeInterval => 0.2f;

        public override void Drop(GameObject obj = null)
        {
            obj = PoolManager.Instance.Get(Key.Trash);
            Vector3 random = new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f));
            Vector3 offset = (_objectStack.Count % 2 == 0) ? -Vector3.left * 0.25f + random : Vector3.left * 0.25f + random;
            obj.transform.position = transform.position + offset;
            obj.SetActive(true);
            _objectStack.Push(obj);
        }
    }
}