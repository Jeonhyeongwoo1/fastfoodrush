using UnityEngine;

namespace FastFoodRush.Scripts.Data
{
    [CreateAssetMenu(fileName = "Burger Machine Data", menuName = "Scriptable Object/Fastfood Data")]
    public class BurgerMachineConfigData : ScriptableObject
    {
        public int DefaultCapacity => _defaultCapacity;
        public float DefaultCreateTime => _defaultCreateTime;
        
        [SerializeField] private int _defaultCapacity;
        [SerializeField] private float _defaultCreateTime;
    }
}