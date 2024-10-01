using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace FastFoodRush.Object
{
    public class UnlockableObject : MonoBehaviour
    {
        public int MoneyNeedToUnlock => _moneyNeedToUnlock;
        public Vector3 GetBuyPointPosition => transform.TransformPoint(_buyPoint);
        public Vector3 GetBuyPointRotation => _buyPointRot;
        public bool IsUnlock => gameObject.activeSelf;

        protected int _unlockLevel = 0;
     
        [SerializeField] private string _id;
        [SerializeField] private Vector3 _buyPoint;
        [SerializeField] private Vector3 _buyPointRot;
        [SerializeField] private int _moneyNeedToUnlock;
        [SerializeField] private Vector3 _punchScale = new Vector3(0.1f, 0.2f, 0.1f);

        public virtual void Unlock(bool animate = true)
        {
            _unlockLevel++;
            
            gameObject.SetActive(true);

            if (!animate)
            {
                return;
            }

            transform.DOPunchScale(_punchScale, 0.3f)
                        .OnComplete(() => transform.localScale = Vector3.one);
        }
    }
}