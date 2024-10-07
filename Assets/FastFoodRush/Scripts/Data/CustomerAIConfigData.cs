using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastFoodRush.Scripts.Data
{
    [CreateAssetMenu(fileName = "Customer AI Data", menuName = "Scriptable Object/Customer Data")]
    public class CustomerAIConfigData : ScriptableObject
    {
        public float MoveSpeed => _moveSpeed;

        [SerializeField] private float _moveSpeed;
    }
}