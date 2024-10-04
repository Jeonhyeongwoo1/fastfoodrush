using DG.Tweening;
using FastFoodRush.Manager;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class FoodPile : Pile
    {
        protected override float _timeInterval => 0.2f;

        protected override void Start()
        {
            RestaurantManager.Instance.Piles.Add(this);
        }

        public override void Drop(GameObject obj = null)
        {
            obj = PoolManager.Instance.Get(PoolKey.Food);
            
            _objectStack.Push(obj);
            int count = _objectStack.Count - 1;
            bool isLeft = count % 2 == 0;
            int depth = count / 2;

            Vector3 offset = RestaurantManager.Instance.GetOffsetByStackType(StackType.Food);
            Vector3 position = transform.position + (isLeft
                ? new Vector3(-offset.x, depth * offset.y, 0)
                : new Vector3(offset.x, depth * offset.y, 0));

            obj.transform.position = position;
            obj.gameObject.SetActive(true);
            AudioManager.Instance.PlaySfX(AudioKey.Pop);
        }
    }
}