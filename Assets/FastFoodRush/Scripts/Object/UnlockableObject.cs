using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FastFoodRush.Manager;
using UnityEngine;

namespace FastFoodRush.Object
{
    public class UnlockableObject : MonoBehaviour
    {
        public Vector3 GetBuyPointPosition => transform.TransformPoint(_buyPoint);
        public Vector3 GetBuyPointRotation => _buyPointRot;
        public bool IsUnlock => gameObject.activeSelf;

        protected int _unlockLevel = 0;
     
        [SerializeField] private Vector3 _buyPoint;
        [SerializeField] private Vector3 _buyPointRot;
        [SerializeField] private Vector3 _punchScale = new Vector3(0.1f, 0.2f, 0.1f);
        
        [SerializeField] protected List<UpgradeableMesh> _updateableMeshList;

        protected virtual void Start()
        {
            var updateableMeshArray = GetComponentsInChildren<UpgradeableMesh>(true);
            _updateableMeshList = new List<UpgradeableMesh>(updateableMeshArray.Length);
            _updateableMeshList = updateableMeshArray.ToList();
        }

        public virtual void Unlock(bool animate = true)
        {
            _unlockLevel++;
            
            if (_unlockLevel > 1)
            {
                UpgradeableMesh();
            }
            else
            {
                gameObject.SetActive(true);
            }

            if (!animate)
            {
                return;
            }

            transform.DOPunchScale(_punchScale, 0.3f)
                        .OnComplete(() => transform.localScale = Vector3.one);
        }

        protected virtual void UpgradeableMesh()
        {
            int count = _updateableMeshList.Count;
            for (int i = 0; i < count; i++)
            {
                UpgradeableMesh upgradeableMesh = _updateableMeshList[i];
                upgradeableMesh.UpdateMesh();
            }
        }
        
        public virtual void LoadMainTutorial() {}
        public virtual void CompleteMainTutorialProgress() {}
    }
}