using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FastFoodRush.Interactable
{
    public class Door : Interactable
    {
        [SerializeField] private Transform _doorTransform;
        [SerializeField] private int _angle = 80;
        
        private Sequence _sequence;

        protected override void OnPlayerEnter(Transform tr)
        {
            base.OnPlayerEnter(tr);
            
            OpenDoor(tr.position);
        }

        protected override void OnPlayerExit(Transform tr)
        {
            base.OnPlayerExit(tr);
            
            CloseDoor();
        }

        [Button]
        public void OpenDoor(Vector3 position)
        {
            Vector3 direction = Vector3.Normalize(position - transform.position);
            float dot = Vector3.Dot(transform.forward, direction);
            float sign = Mathf.Sign(dot);

            _sequence = DOTween.Sequence();
            _sequence.Append(_doorTransform.DOLocalRotate(new Vector3(0, _angle * sign, 0), 0.5f, RotateMode.LocalAxisAdd));
        }

        public void CloseDoor()
        {
            _sequence = DOTween.Sequence();
            _sequence.SetDelay(0.3f);
            _sequence.Append(_doorTransform.DOLocalRotate(Vector3.zero, 0.5f).SetEase(Ease.OutBounce));
        }
    }
}
