using System;
using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Controller;
using UnityEngine;
using FastFoodRush.Tutorial;

namespace FastFoodRush.Manager
{
    public enum MainTutorialType
    {
        None,
        RestaurantCountTable = 100,
        FoodMachine = 200,
        FirstSeat = 300,
        HR = 400,
        GM = 500,
        DriveThruCounterTable = 600,
        PackingTable = 700,
        SecondSeat = 800
    }
    
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<TutorialManager>();
                }

                return _instance;
            }
        }

        public int MainTutorialStep
        {
            get => _mainTutorialStep;
            private set
            {
                SaveSystem.SaveMainTutorialStep(value);
                _mainTutorialStep = value;
            }
        }

        private static TutorialManager _instance;
        
        public static Action onExcuteTutorial;
        public static Action onCompletedTutorial;
        
        private int _mainTutorialStep = 0;
        private PlayerController _player;
        private BaseTutorial _loadedMainTutorial;
        private Transform _tutorialTarget;
        
        private void Start()
        {
            _player = FindObjectOfType<PlayerController>();
            _mainTutorialStep = SaveSystem.GetMainTutorialStep();
        }

        public bool CheckExecutedMainTutorialProgress(MainTutorialType mainTutorialType)
        {
            if (MainTutorialStep >= (int) mainTutorialType)
            {
                Debug.Log($"is already play tutorial {mainTutorialType} / current step {MainTutorialStep}");
                return true;
            }

            return false;
        }

        public void SetTutorialTarget(Transform tutorialTarget)
        {
            _tutorialTarget = tutorialTarget;
        }

        public bool CheckMainTutorialCompletion(MainTutorialType mainTutorialType)
        {
            if (_loadedMainTutorial == null)
            {
                return false;
            }

            MainTutorialStep = (int)_loadedMainTutorial.MainTutorialType;
            _loadedMainTutorial.CompletedTutorial();
            onCompletedTutorial?.Invoke();
            return true;
        }
        
        public void LoadTutorial(MainTutorialType mainTutorialType)
        {
            if (mainTutorialType == MainTutorialType.None)
            {
                Debug.Log($"tutorial type is none");
                return;
            }
            
            switch (mainTutorialType)
            {
                case MainTutorialType.RestaurantCountTable:
                case MainTutorialType.FoodMachine:
                case MainTutorialType.FirstSeat:
                case MainTutorialType.HR:
                case MainTutorialType.GM:
                case MainTutorialType.DriveThruCounterTable:
                case MainTutorialType.PackingTable:
                case MainTutorialType.SecondSeat:
                    CheckMainTutorialCompletion(MainTutorialType.None);
                    var tutorial = Resources.Load<BaseTutorial>($"Tutorial/MainTutorial") as MainTutorial;
                    var mainTutorial = Instantiate(tutorial);
                    mainTutorial.Initialized(_player, _tutorialTarget, mainTutorialType);
                    mainTutorial.ExecuteTutorial();
                    onExcuteTutorial?.Invoke();
                    _loadedMainTutorial = mainTutorial;
                    break;
                default:
                    break;
            }
        }

    }
}
