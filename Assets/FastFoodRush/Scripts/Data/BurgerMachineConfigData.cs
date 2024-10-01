using UnityEngine;
using UnityEngine.Serialization;

namespace FastFoodRush.Scripts.Data
{
    [CreateAssetMenu(fileName = "Burger Machine Data", menuName = "Scriptable Object/Fastfood Data")]
    public class BurgerMachineConfigData : ScriptableObject
    {
        public int Capacity => _capacity;
        public float CreateTime => _createTime;
        
        [SerializeField] private int _capacity;
        [SerializeField] private float _createTime;
    }
}