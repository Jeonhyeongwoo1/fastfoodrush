using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FastFoodRush.Object
{
    public class FlippingObject : MonoBehaviour
    {
        private Sequence _sequence;
        private Vector3 _originPos;

        private void Start()
        {
            _originPos = transform.position;
        }

        private void OnEnable()
        {
            DoFlip();
        }

        private void OnDisable()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
                _sequence = null;
            }

            transform.position = _originPos;
        }

        private void DoFlip()
        {
            float delay = Random.Range(0.6f, 1.5f);
            _sequence = DOTween.Sequence();
            _sequence.SetDelay(delay);
            _sequence.Append(transform.DOJump(transform.position, 1, 1, 1f));
            _sequence.Join(transform.DOLocalRotate(new Vector3(0, 0, 180f), 0.5f, RotateMode.LocalAxisAdd));
            _sequence.OnComplete(() => DoFlip());
        }
    } 
}