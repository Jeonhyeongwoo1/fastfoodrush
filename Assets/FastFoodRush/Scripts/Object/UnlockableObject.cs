using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastFoodRush.Object
{
    public class UnlockableObject : MonoBehaviour
    {
        [SerializeField] private string _id;

        [SerializeField] private Vector3 _buyPoint;
        [SerializeField] private int _moneyNeedToUnlock;
        public int MoneyNeedToUnlock => _moneyNeedToUnlock;
        public Vector3 GetBuyPoint => transform.TransformPoint(_buyPoint);
        
        public void Unlock()
        {
            gameObject.SetActive(true);
        }
    }
}