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
        SecondSeat = 500,
        GM = 600,
        DriveThruCounterTable = 700,
        PackingTable = 800,
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
        
        private static TutorialManager _instance;

        public int MainTutorialStep
        {
            get => _mainTutorialStep;
            private set
            {
                SaveSystem.SaveMainTutorialStep(value);
                _mainTutorialStep = value;
            }
        }

        public static Action onCompletedMainTutorialAction;

        [SerializeField] private bool _useTutorial;
        
        private int _mainTutorialStep = 0;
        private int _trashTutorialStep = 0;
        
        private PlayerController _player;
        private BaseTutorial _loadedMainTutorial;
        private Transform[] _tutorialTarget;
        
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

        public void SetTutorialTarget(params Transform[] tutorialTarget)
        {
            _tutorialTarget = tutorialTarget;
        }

        public bool IsRunningTutorial()
        {
            return _loadedMainTutorial != null && !_loadedMainTutorial.IsAllCompletedTutorial;
        }

        public bool CheckMainTutorialCompletion(MainTutorialType mainTutorialType)
        {
            if (_loadedMainTutorial == null || _loadedMainTutorial.MainTutorialType != mainTutorialType)
            {
                return false;
            }

            if (!_loadedMainTutorial.IsAllCompletedTutorial)
            {
                return false;
            }

            MainTutorialStep = (int)_loadedMainTutorial.MainTutorialType;
            _loadedMainTutorial.CompletedTutorial();
            onCompletedMainTutorialAction?.Invoke();
            _loadedMainTutorial = null;
            return true;
        }

        public void CompleteMainTutorialDepth(MainTutorialType mainTutorialType)
        {
            if (_loadedMainTutorial == null || _loadedMainTutorial.MainTutorialType != mainTutorialType)
            {
                return;
            }

            _loadedMainTutorial.CompletedTutorialDepth();
        }

        public void LoadTutorial(MainTutorialType mainTutorialType)
        {
            if (!_useTutorial)
            {
                return;
            }
            
            if (mainTutorialType == MainTutorialType.None)
            {
                Debug.Log($"tutorial type is none");
                return;
            }

            if (_loadedMainTutorial != null)
            {
                CheckMainTutorialCompletion(_loadedMainTutorial.MainTutorialType);
            }
            
            Debug.Log($"load tutorial {mainTutorialType}");
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
                    var tutorial = Resources.Load<BaseTutorial>($"Tutorial/MainTutorial") as MainTutorial;
                    var mainTutorialObj = Instantiate(tutorial);
                    mainTutorialObj.Initialized(_player, mainTutorialType, _tutorialTarget);
                    mainTutorialObj.ExecuteTutorial();
                    _loadedMainTutorial = mainTutorialObj;
                    break;
                default:
                    break;
            }
        }

    }
}
