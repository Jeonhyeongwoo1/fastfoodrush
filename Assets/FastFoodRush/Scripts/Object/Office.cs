using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Manager;
using FastFoodRush.Object;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class Office : UnlockableObject
    {
        [SerializeField] private MainTutorialType _mainTutorialType;
        
        public override void MainTutorialProgress()
        {
            TutorialManager tutorialManager = TutorialManager.Instance;
            bool isExecutedTutorial =  tutorialManager.CheckExecutedMainTutorialProgress(_mainTutorialType);
            if (!isExecutedTutorial)
            {
                tutorialManager.SetTutorialTarget(RestaurantManager.Instance.UnlockableBuyer.transform);
                tutorialManager.LoadTutorial(_mainTutorialType);
            }
        }
    }
}