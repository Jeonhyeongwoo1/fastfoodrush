using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Interactable;
using UnityEngine;
using UnityEngine.AI;

namespace FastFoodRush.Controller
{
    public class BaseController : MonoBehaviour
    {
        [SerializeField] protected Animator _animator;
        [SerializeField] protected WobblingStack _wobblingStack;
        [SerializeField] protected NavMeshAgent _agent;
        [SerializeField] private Transform _leftHandPoint;
        [SerializeField] private Transform _rightHandPoint;

        protected int _isMovingHash;

        protected virtual void Start()
        {
            _isMovingHash = Animator.StringToHash("IsMoving");
        }

        protected bool HasArrived()
        {
            if (!_agent.pathPending)
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected void OnAnimatorIK(int layerIndex)
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
    }
}