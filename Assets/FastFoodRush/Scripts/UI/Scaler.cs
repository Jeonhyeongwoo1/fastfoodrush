using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FastFoodRush.UI
{
    public class Scaler : MonoBehaviour
    {
        [SerializeField] private bool _useAnimation = true;

        private Sequence _sequence;

        private void OnEnable()
        {
            if (_useAnimation)
            {
                _sequence = DOTween.Sequence();
                _sequence.Append(transform.DOScale(Vector3.one * 1.2f, .7f));
                _sequence.SetLoops(-1, LoopType.Yoyo);
            }
        }

        private void OnDisable()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
                _sequence = null;
            }
        }
    }   
}
