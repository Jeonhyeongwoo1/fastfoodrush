using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FastFoodRush.Object
{
    public class Indicator : MonoBehaviour
    {
        [SerializeField] private float _endValue = 2;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private Vector3 _arrowOffset;
        
        public void DoIndicate(Transform arrowtarget)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            transform.position = arrowtarget.position + _arrowOffset;
            transform.DOLocalMoveY(_endValue, _duration).SetLoops(-1, LoopType.Yoyo);
        }
    }
}