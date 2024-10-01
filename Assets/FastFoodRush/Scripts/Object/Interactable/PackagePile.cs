using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FastFoodRush.Manager;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class PackagePile : Pile
    {
        protected override float _timeInterval => 0.3f;
        
        protected override void Start()
        {
            RestaurantManager.Instance.Piles.Add(this);
        }
        
        public override void Drop(GameObject obj = null)
        {
            Vector3 endValue = transform.position +
                               new Vector3(0, RestaurantManager.Instance.GetOffsetByStackType(StackType.Package).y, 0) *
                               _objectStack.Count;
            _objectStack.Push(obj);
            obj.transform.DOJump(endValue, 3.5f, 1, _timeInterval);
        }
    }
}