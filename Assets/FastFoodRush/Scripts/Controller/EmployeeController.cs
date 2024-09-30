using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace FastFoodRush.Controller
{
    public class EmployeeController : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;

        private int _isMovingHash;
        
        // Start is called before the first frame update
        private void Start()
        {
            _isMovingHash = Animator.StringToHash("IsMoving");
        }

        private void Update()
        {
            
        }

        public void Initialize(Vector3 spawnPosition)
        {
            transform.position = spawnPosition;
            gameObject.SetActive(true);
        }
    }
}
