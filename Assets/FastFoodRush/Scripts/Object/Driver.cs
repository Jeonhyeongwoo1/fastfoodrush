using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;


namespace FastFoodRush.Object
{
    public interface IOrderable
    {
        public bool IsReadyOrder { get; }
        public int 
            RemainOrderCount { get; }
        public int Height { get; }
        public int OrderCount { get; }
        public Transform Transform { get; }
        public bool TryReceivedOrderInfo(GameObject obj);
        public void UpdateQueuePosition(Vector3 position);
    }
    
    public class Driver : MonoBehaviour, IOrderable
    {
        public bool IsReadyOrder { get; private set; }
        public int RemainOrderCount => _orderCount - _currentReceivedOrderCount;
        public int Height => _height;
        public int OrderCount => _orderCount;
        public Transform Transform => transform;

        private int _orderCount;
        private Vector3 _despawnPosition;
        private Vector3 _queuePointPosition;
        private List<Vector3> _movePositionList;
        private int _currentReceivedOrderCount;
        private int _height;

        private void Start()
        {
            StartCoroutine(MoveToWaittingPointCor());
        }

        private void OnDisable()
        {
            IsReadyOrder = false;
        }

        private IEnumerator MoveToWaittingPointCor()
        {
            int count = _movePositionList.Count;
            for (int i = 0; i < count; i++)
            {
                yield return MoveToCor(_movePositionList[i]);
            }

            _movePositionList.Clear();
        }

        private IEnumerator MoveToCor(Vector3 destinationPosition, Action done =null)
        {
            const float delta = 6;
            IsReadyOrder = false;
            while (true)
            {
                Vector3 myPos = transform.position;
                if (Math.Abs((destinationPosition - myPos).magnitude - 0.1f) <= 0.1f)
                {
                    break;
                }

                Vector3 direction = (destinationPosition - myPos).normalized;
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                Quaternion myRot = transform.rotation;
                transform.rotation = Quaternion.Lerp(myRot, rotation, Time.deltaTime * delta);
                transform.position = Vector3.MoveTowards(myPos, destinationPosition, Time.deltaTime * delta);
                
                yield return null;
            }

            IsReadyOrder = true;
            done?.Invoke();
        }
        
        public void Spawn(Vector3 spawnPosition, int maxFoodCapacity, Vector3 despawnPosition, List<Vector3> movePointList)
        {
            movePointList.Reverse();
            _movePositionList = movePointList;
            _orderCount = maxFoodCapacity;
            _despawnPosition = despawnPosition;
            
            transform.position = spawnPosition;
            transform.eulerAngles = new Vector3(0, -90, 0);
            gameObject.SetActive(true);
        }

        public void Leave()
        {
            StartCoroutine(MoveToCor(_despawnPosition, () =>
            {
                IsReadyOrder = false;
                gameObject.SetActive(false);
            }));
        }
        
        public bool TryReceivedOrderInfo(GameObject obj)
        {
            if (RemainOrderCount == 0)
            {
                return false;
            }
            
            _height++;
            Sequence seq = DOTween.Sequence();
            seq.Append(obj.transform.DOJump(transform.position, 4, 1, 0.3f));
            seq.OnComplete(() =>
            {
                obj.SetActive(false);
                _currentReceivedOrderCount++;
            });

            return true;
        }

        public void UpdateQueuePosition(Vector3 position)
        {
            StartCoroutine(MoveToCor(position));
        }
    }
}