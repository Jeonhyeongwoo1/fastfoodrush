using DG.Tweening;
using FastFoodRush.Manager;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class FoodPile : Pile
    {
        protected override float _timeInterval => 0.2f;
        
        public override void Drop()
        {
            GameObject go = PoolManager.Instance.Get(Key.Food);
            
            _objectStack.Push(go);
            int count = _objectStack.Count - 1;
            bool isLeft = count % 2 == 0;
            int depth = count / 2;

            Vector3 offset = RestaurantManager.Instance.GetOffsetByStackType(StackType.Food);
            Vector3 position = transform.position + (isLeft
                ? new Vector3(-offset.x, depth * offset.y, 0)
                : new Vector3(offset.x, depth * offset.y, 0));

            go.transform.position = position;
            go.gameObject.SetActive(true);
        }
    }
}