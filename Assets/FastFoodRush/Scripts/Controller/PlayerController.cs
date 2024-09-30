using System;
using FastFoodRush.Interactable;
using FastFoodRush.Manager;
using UnityEngine;

namespace FastFoodRush.Controller
{
    public class PlayerController : MonoBehaviour
    {
        public WobblingStack Stack => _wobblingStack;
        public int PlayerCapacity => _playerCapacity;
        
        [SerializeField] private Animator _animator;
        [SerializeField] private WobblingStack _wobblingStack;
        [SerializeField] private Transform _leftHandPoint;
        [SerializeField] private Transform _rightHandPoint;
        
        private float _moveSpeed;
        private int _playerCapacity;
        private Vector3 _movement;
        private CharacterController _characterController;

        private int _isMovingHash;
        
        private void OnAnimatorIK(int layerIndex)
        {
            if (_wobblingStack.StackCount == 0)
            {
                return;
            }
            
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHandPoint.position);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandPoint.position);
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandPoint.rotation);
            _animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHandPoint.rotation);
        }

        private void Awake()
        {
            _characterController = GetComponentInChildren<CharacterController>();
            _isMovingHash = Animator.StringToHash("IsMoving");
        }
        
        private void OnEnable()
        {
            RestaurantManager.Instance.onUpgradedAbility += OnUpgradeAbility;
        }

        private void OnDisable()
        {
            RestaurantManager.Instance.onUpgradedAbility -= OnUpgradeAbility;
        }

        private void Start()
        {
            _moveSpeed = RestaurantManager.Instance.GetStatusValue(AbilityType.PlayerSpeed);
            _playerCapacity = (int)RestaurantManager.Instance.GetStatusValue(AbilityType.PlayerCapacity);
        }

        private void OnUpgradeAbility(AbilityType abilityType, float ability)
        {
            switch (abilityType)
            {
                case AbilityType.PlayerSpeed:
                    _moveSpeed = ability;
                    break;
                case AbilityType.PlayerCapacity:
                    _playerCapacity = (int) ability;
                    break;
                case AbilityType.PlayerProfit:
                    break;
            }
        }

        private void Update()
        {
            _movement.x = SimpleInput.GetAxis("Horizontal");
            _movement.z = SimpleInput.GetAxis("Vertical");
            _movement = (Quaternion.Euler(new Vector3(0, 45, 0)) * _movement).normalized;
            bool isMoving = _movement.x != 0 || _movement.z != 0;

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