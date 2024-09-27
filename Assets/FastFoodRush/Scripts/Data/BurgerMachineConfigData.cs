using UnityEngine;
using UnityEngine.Serialization;

namespace FastFoodRush.Scripts.Data
{
    [CreateAssetMenu(fileName = "Burger Machine Data", menuName = "Scriptable Object/Fastfood Data")]
    public class BurgerMachineConfigData : ScriptableObject
    {
        public int capacity => _capacity;
        public int createTime => _createTime;
        
        [SerializeField] private int _capacity;
        [SerializeField] private int _createTime;
    }
}