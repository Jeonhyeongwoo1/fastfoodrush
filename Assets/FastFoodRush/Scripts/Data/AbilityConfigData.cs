using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastFoodRush.Scripts.Data
{
    [CreateAssetMenu(fileName = "Ability Data", menuName = "Scriptable Object/Ability Data")]
    public class AbilityConfigData : ScriptableObject
    {
        public int FirstUpgradePrice => _firstUpgradePrice;
        public int UpgradePrice => _upgradePrice;
        public int MaxLevel => _maxLevel; 
        
        [SerializeField] int _maxLevel;
        [SerializeField] private int _firstUpgradePrice;
        [SerializeField] private int _upgradePrice; 
    }
}