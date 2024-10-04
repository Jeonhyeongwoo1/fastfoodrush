using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FastFoodRush.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FastFoodRush.Manager
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Transform _punchAnimationTransform;
        [SerializeField] private ScreenFader _screenFader;
        
        private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            
            _startButton.onClick.RemoveAllListeners();
            _startButton.onClick.AddListener(()=> AudioManager.Instance.PlaySfX(AudioKey.Magical));
            _startButton.onClick.AddListener(OnClickStartButton);

            AudioManager.Instance.PlayBGM(AudioKey.BGM);
            
            Sequence sequence = DOTween.Sequence();
            sequence.SetDelay(.5f);
            sequence.Append(_punchAnimationTransform.DOScale(Vector3.one * 1.1f, 0.5f));
            sequence.SetLoops(-1, LoopType.Yoyo);
        }

        private void OnClickStartButton()
        {
            DOTween.KillAll();
            _startButton.interactable = false;
            _screenFader.FadeOut(() =>
            {
                string id = SaveSystem.LoadLastPlayRestaurantId();
                if (string.IsNullOrEmpty(id))
                {
                    int index = SceneManager.GetActiveScene().buildIndex;
                    SceneManager.LoadScene(++index);
                }
                else
                {
                    SceneManager.LoadScene(id);
                }
            });
        }

        [Sirenix.OdinInspector.Button]
        public void DeleteAllData()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
