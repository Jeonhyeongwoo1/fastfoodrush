using System;
using SimpleInputNamespace;
using UnityEngine;

namespace FastFoodRush.Controller
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _moveSpeed;
        
        private Vector3 _movement;
        private CharacterController _characterController;

        private int _isMovingHash;
        
        private void Start()
        {
            _characterController = GetComponentInChildren<CharacterController>();

            _isMovingHash = Animator.StringToHash("IsMoving");
        }

        private void Update()
        {
            
            _movement.x = SimpleInput.GetAxis("Horizontal");
            _movement.z = SimpleInput.GetAxis("Vertical");
            _movement = (Quaternion.Euler(new Vector3(0, 45, 0)) * _movement).normalized;
            bool isMoving = _movement.x != 0 || _movement.z != 0;
            Debug.Log($"{_movement.x} / {_movement.z}");

            if (isMoving)
            {
                _characterController.Move(_movement * (Time.deltaTime * _moveSpeed));
            
                Quaternion lookRotation = Quaternion.LookRotation(_movement, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * 10);
            }

            _animator.SetBool(_isMovingHash, isMoving);
        }
        
        
        public void OnStep(AnimationEvent animationEvent)
        {
            // if (animationEvent.animatorClipInfo.weight < 0.5f) return;
            //
            // audioSource.clip = footsteps[Random.Range(0, footsteps.Length)];
            // audioSource.Play();
        }
    }
}