using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastFoodRush.Object
{
    public class UnlockableObject : MonoBehaviour
    {
        [SerializeField] private string _id;

        [SerializeField] private Vector3 _buyPoint;
        [SerializeField] private Vector3 _buyPointRot;
        [SerializeField] private int _moneyNeedToUnlock;
        public int MoneyNeedToUnlock => _moneyNeedToUnlock;
        public Vector3 GetBuyPointPosition => transform.TransformPoint(_buyPoint);
        public Vector3 GetBuyPointRotation => _buyPointRot;
        public bool IsUnlock => gameObject.activeSelf;
        
        public virtual void Unlock()
        {
            gameObject.SetActive(true);
        }
    }
}