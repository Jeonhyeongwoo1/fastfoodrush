using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FastFoodRush.Manager;
using FastFoodRush.Object;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class PackingTable : UnlockableObject
    {
        [SerializeField] private ObjectStack _objectStack;
        [SerializeField] private List<GameObject> _foodObjectList;
        [SerializeField] private PackagePile _packagePile;
        [SerializeField] private Transform _packageTransform;
        [SerializeField] private GameObject _workerObj;
        
        private Sequence _sequence;

        private void Update()
        {
            MakePackage();
        }

        protected override void UpgradeableMesh()
        {
            base.UpgradeableMesh();

            if (_workerObj)
            {
                _workerObj.SetActive(_unlockLevel >= 2);
            }
        }

        public override void MainTutorialProgress()
        {
            
            TutorialManager tutorialManager = TutorialManager.Instance;
            bool isExecutedTutorial =  tutorialManager.CheckExecutedMainTutorialProgress(MainTutorialType.PackingTable);
            if (!isExecutedTutorial)
            {
                tutorialManager.SetTutorialTarget(RestaurantManager.Instance.UnlockableBuyer.transform);
                tutorialManager.LoadTutorial(MainTutorialType.PackingTable);
            }
        }

        private void MakePackage()
        {
            if (_objectStack.StackCount == 0 || _sequence != null) 
            {
                return;
            }
            
            GameObject targetObj = _foodObjectList.Find(v => !v.activeSelf);
            if (targetObj == null)
            {
                return;
            }

            GameObject obj = _objectStack.GetStackObject();
            Vector3 endValue = targetObj.transform.position;
            _sequence = DOTween.Sequence();
            _sequence.Append(obj.transform.DOJump(endValue, 2, 1, 0.2f));
            _sequence.OnComplete(() =>
            {
                targetObj.SetActive(true);
                obj.SetActive(false);
                _sequence = null;

                bool isAllActivated = _foodObjectList.TrueForAll((v) => v.activeSelf);
                if (isAllActivated)
                {
                    GameObject packageObj = PoolManager.Instance.Get(Key.Package);
                    packageObj.transform.position = _packageTransform.position;
                    packageObj.SetActive(true);
                    _packagePile.Drop(packageObj);
                    _foodObjectList.ForEach(v=> v.SetActive(false));
                }
            });
        }
    }
}