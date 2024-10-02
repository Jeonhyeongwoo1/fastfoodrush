using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FastFoodRush.Utils
{
    public class ScreenFader : MonoBehaviour
    {
        [SerializeField] private Image _fadeImage;
        [SerializeField] private float _fadeDuration;
        
        public void FadeOut(Action done)
        {
            _fadeImage.raycastTarget = true;
            _fadeImage.DOFade(1, _fadeDuration).OnComplete(() => done?.Invoke());
        }

        public void FadeIn(Action done)
        {
            _fadeImage.raycastTarget = false;
            _fadeImage.DOFade(0, _fadeDuration).OnComplete(() => done?.Invoke());
        }
    }
}