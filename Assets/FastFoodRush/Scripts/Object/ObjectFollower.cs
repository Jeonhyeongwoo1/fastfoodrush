using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastFoodRush.Object
{
    public class ObjectFollower : MonoBehaviour
    {
        [SerializeField] private Transform _follower;
        [SerializeField] private bool _useFixedUpdate;
        [SerializeField] private float _followSpeed;
        [SerializeField] private Vector3 _offset;
        
        private void FixedUpdate()
        {
            if (!_useFixedUpdate)
            {
                return;
            }

            Vector3 position = Vector3.Lerp(transform.position, _follower.position, Time.fixedTime * _followSpeed);
            transform.position = position + _offset;
        }
    }
}