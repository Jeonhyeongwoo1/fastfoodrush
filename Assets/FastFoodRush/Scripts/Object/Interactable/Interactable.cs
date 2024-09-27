using System;
using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Controller;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class Interactable : MonoBehaviour
    {
        protected PlayerController _player;
        private const string PlayerTag = "Player";
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(PlayerTag))
            {
                return;
            }

            _player = other.GetComponent<PlayerController>();
            OnPlayerEnter(other.transform);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(PlayerTag))
            {
                return;
            }

            if (_player != null)
            {
                _player = null;
            }
            
            OnPlayerEnter(other.transform);
        }

        protected virtual void OnPlayerEnter(Transform tr) {}
        protected virtual void OnPlayerExit(Transform tr) {}
    }
}