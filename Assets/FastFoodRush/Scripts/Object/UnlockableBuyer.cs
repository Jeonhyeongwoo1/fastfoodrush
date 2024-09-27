using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FastFoodRush.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FastFoodRush.Object
{
    public class UnlockableBuyer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _paidMoneyText;
        [SerializeField] private Image _prograssImage;
        [SerializeField] private float _duration = 3;
        [SerializeField] private float _timeInterval = 0.02f;
        
        private const string playerTag = "Player";

        private UnlockableObject _unlockableObject;
        private int _moneyNeedToUnlock;
        private int _paidAmount;
        private Coroutine _payCor;

        public void Initialize(UnlockableObject unlockableObject, int paidAmount)
        {
            _unlockableObject = unlockableObject;
            _moneyNeedToUnlock = _unlockableObject.MoneyNeedToUnlock;
            _paidAmount = paidAmount;

            _paidMoneyText.text = _moneyNeedToUnlock.ToString();
            _prograssImage.fillAmount = 0;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag))
            {
                return;
            }

            // if (RestaurantManager.Instance.Moneny == 0)
            // {
            //     return;
            // }

            _payCor = StartCoroutine(PayCor(other.transform));
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag(playerTag))
            {
                return;
            }

            if (_payCor != null)
            {
                StopCoroutine(_payCor);
                _payCor = null;
            }
        }

        private void UpdatePaidMoney(int amount)
        {
            _paidAmount += amount;
            _paidMoneyText.text = (_moneyNeedToUnlock - _paidAmount).ToString();
            _prograssImage.fillAmount = (float)_paidAmount / _moneyNeedToUnlock;
        }

        private IEnumerator PayCor(Transform playerTransform)
        {
            RestaurantManager manager = RestaurantManager.Instance;
            while (manager.Moneny > 0 && _paidAmount < _moneyNeedToUnlock)
            {
                float num = _moneyNeedToUnlock * _timeInterval / _duration;
                float value = Mathf.Min(manager.Moneny, num);
                int amount = Mathf.Max(1, Mathf.RoundToInt(value));
                
                UpdatePaidMoney(amount);
                RestaurantManager.Instance.Moneny -= amount;

                GameObject moneyObj = PoolManager.Instance.Get(Key.Money);
                moneyObj.transform.position = playerTransform.position + new Vector3(0, 1f, 0);
                moneyObj.SetActive(true);
                moneyObj.transform.DOJump(transform.position, 2, 1, 0.15f)
                    .OnComplete(() => moneyObj.SetActive(false));
                
                yield return new WaitForSeconds(_timeInterval);
            }

            if (_paidAmount >= _moneyNeedToUnlock)
            {
                RestaurantManager.Instance.BuyUnlockableObject();
            }
        }
    }
}