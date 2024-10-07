using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FastFoodRush.Manager;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class MoneyPile : Pile
    {
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private Vector3 _spacing;
        [SerializeField] private int _xCount = 2;
        [SerializeField] private int _zCount = 4;
        [SerializeField] private int _oneDepthMaxCount = 8;
        
        protected override float _timeInterval => 0.15f;
        private readonly int _maxMoneyObjectStackCount = 200;
        private int _totalMoney;
        private List<GameObject> _moneyObjectList = new ();

        protected override void Update()
        {
        }

        protected override void OnPlayerEnter(Transform tr)
        {
            base.OnPlayerEnter(tr);
            
            Drop();
        }
        
        private IEnumerator GetMoney()
        {
            if (_totalMoney == 0 || _moneyObjectList.Count == 0)
            {
                yield break;
            }
            
            int count = _moneyObjectList.Count;
            int totalMoney = RestaurantManager.Instance.Money + _totalMoney;
            
            _totalMoney = 0;

            float elasped = 0;
            float duration = 1.5f;
            for (int i = count - 1; i >= 0; i--)
            {
                elasped += Time.deltaTime;

                if (elasped > duration)
                {
                    break;
                }

                if (RestaurantManager.Instance.Money == totalMoney)
                {
                    break;
                }

                GameObject obj = _moneyObjectList[i];
                Vector3 endValue = _player.transform.position;
                obj.transform.DOJump(endValue + Vector3.up, 3, 1, 0.15f)
                            .OnComplete(() => obj.SetActive(false));
                _moneyObjectList.Remove(obj);
                RestaurantManager.Instance.Money++;
                AudioManager.Instance.PlaySFX(AudioKey.Money);
                yield return null;
            }
            
            //나머지는 모두 넣기
            for (int i = 0; i < _moneyObjectList.Count; i++)
            {
                GameObject obj = _moneyObjectList[i];
                Vector3 endValue = _player.transform.position;
                obj.transform.DOJump(endValue + Vector3.up, 3, 1, 0.15f)
                    .OnComplete(() => obj.SetActive(false));
            }

            _moneyObjectList.Clear();
            RestaurantManager.Instance.Money = totalMoney;
        }
        
        public override void Drop(GameObject obj = null)
        {
            StartCoroutine(GetMoney());
        }
        
        [Button]
        public void AddMoney(int price)
        {
            _totalMoney += price;

            if (_moneyObjectList.Count == _maxMoneyObjectStackCount)
            {
                AudioManager.Instance.PlaySFX(AudioKey.Money);
                return;
            }
            
            int totalMoneyCount = _totalMoney;
            totalMoneyCount = _maxMoneyObjectStackCount < totalMoneyCount ? _maxMoneyObjectStackCount : totalMoneyCount;
            totalMoneyCount = totalMoneyCount <= 3 && _totalMoney >= 1 ? 3 : totalMoneyCount;
            
            Vector3 center = new Vector3(_width * 0.5f, 0, _height * 0.5f);
            int xIndex = 0;
            int zIndex = 0;
            int yIndex = 0;
            int half = (int)(_oneDepthMaxCount * 0.5);
            bool isLeft = true;
            for (int i = 0; i < totalMoneyCount; i++)
            {
                GameObject obj = PoolManager.Instance.Get(PoolKey.Money);
                if (zIndex >= half)
                {
                    isLeft = !isLeft;
                    zIndex = 0;
                    xIndex = isLeft ? 0 : 1;
                }
                
                if (i % _oneDepthMaxCount == 0 && i != 0)
                {
                    yIndex++;
                }
                
                float x = -center.x + (_width + _spacing.x) / _xCount * xIndex;
                float z = center.z + -((_height + _spacing.z) / _zCount * zIndex);
                float y = _spacing.y * yIndex;
                zIndex++;
                
                obj.transform.position = transform.position + new Vector3(x, y, z);
                obj.SetActive(true);
                _moneyObjectList.Add(obj);
            }
            
            AudioManager.Instance.PlaySFX(AudioKey.Money);
        }
    }
}