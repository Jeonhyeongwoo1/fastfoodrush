using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FastFoodRush.Object
{
    public class MovingObject : MonoBehaviour
    {
        [SerializeField] private Vector3 _endPos;
        private Vector3 _originPos;

        public void Moving(float duration)
        {
            gameObject.SetActive(true);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOLocalMove(_endPos, duration));
            sequence.SetLoops(-1, LoopType.Restart);
        }
        
        private void OnDisable()
        {
            transform.position = _originPos;
        }

        private void Start()
        {
            _originPos = transform.position;
        }
    }
}
