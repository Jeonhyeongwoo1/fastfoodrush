using DG.Tweening;
using FastFoodRush.Manager;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class FoodPile : Pile
    {
        protected override float _timeInterval => 0.2f;

        [SerializeField] private bool _isCenterAxisX;
        [SerializeField] private bool _useCustomOffset;
        [SerializeField] private Vector3 _customOffset = new Vector3(0, 0.1f, 0);
        
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

            Vector3 offset = _useCustomOffset
                ? _customOffset
                : RestaurantManager.Instance.GetOffsetByStackType(StackType.Food);
            Vector3 position = Vector3.zero;
            if (_isCenterAxisX)
            {
                position = transform.position + (isLeft
                    ? new Vector3(-offset.x, depth * offset.y, 0)
                    : new Vector3(offset.x, depth * offset.y, 0));
                
            }
            else
            {
                position = transform.position + (isLeft
                    ? new Vector3(0, depth * offset.y, -offset.z)
                    : new Vector3(0, depth * offset.y, offset.z));
            }
            
            obj.transform.position = position;
            obj.gameObject.SetActive(true);
        }
    }
}