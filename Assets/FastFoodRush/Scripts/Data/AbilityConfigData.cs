using System;
using System.Collections;
using System.Collections.Generic;
using FastFoodRush.Manager;
using UnityEngine;
using UnityEngine.Serialization;

namespace FastFoodRush.Scripts.Data
{
    [CreateAssetMenu(fileName = "Ability Data", menuName = "Scriptable Object/Ability Data")]
    public class AbilityConfigData : ScriptableObject
    {
        public int FirstUpgradePrice => _firstUpgradePrice;
        public int UpgradePrice => _upgradePrice;
        public int MaxLevel => _maxLevel;
        public float DefaultPlayerMoveSpeed => _defaultPlayerMoveSpeed;
        public int DefaultPlayerCapacity => _defaultPlayerCapacity;
        public float DefaultPlayerProfit => _defaultPlayerProfit;
        
        [SerializeField] int _maxLevel;
        [SerializeField] private int _firstUpgradePrice;
        [SerializeField] private int _upgradePrice;
        
        [SerializeField] private float _defaultPlayerMoveSpeed;
        [SerializeField] private int _defaultPlayerCapacity;
        [SerializeField] private int _defaultPlayerProfit;
        [SerializeField] private float _defaultEmployeeMoveSpeed;
        [SerializeField] private int _defaultEmployeeCapacity;

        [SerializeField] private float _playerMoveSpeedUpgradeValue;
        [SerializeField] private float _playerCapacityUpgradeValue;
        [SerializeField] private int _playerProfitUpgradeValue;
        [SerializeField] private float _employeeMoveSpeedUpgradeValue;
        [SerializeField] private int _employeeCapacityUpgradeValue;
        
        public float GetStatusValue(AbilityType abilityType, int level)
        {
            switch (abilityType)
            {
                case AbilityType.PlayerSpeed:
                    return level * _playerMoveSpeedUpgradeValue + _defaultPlayerMoveSpeed;
                case AbilityType.PlayerCapacity:
                    return level * _playerCapacityUpgradeValue + _defaultPlayerCapacity;
                case AbilityType.PlayerProfit:
                    return level * _playerProfitUpgradeValue + _defaultPlayerProfit;
                case AbilityType.EmployeeSpeed:
                    return level * _employeeMoveSpeedUpgradeValue + _defaultEmployeeMoveSpeed;
                case AbilityType.EmployeeCapacity:
                    return level * _employeeCapacityUpgradeValue + _defaultEmployeeCapacity;
                case AbilityType.EmployeeAmount:
                    return 0;
                default:
                    return -1;
            }
        }
        
        public float GetDefaultAbilityValue(AbilityType abilityType)
        {
            switch (abilityType)
            {
                case AbilityType.PlayerSpeed:
                    return _defaultPlayerMoveSpeed;
                case AbilityType.PlayerCapacity:
                    return _defaultPlayerCapacity;
                case AbilityType.PlayerProfit:
                    return _defaultPlayerProfit;
                case AbilityType.EmployeeCapacity:
                    return _defaultEmployeeCapacity;
                case AbilityType.EmployeeSpeed:
                    return _defaultEmployeeMoveSpeed;
                case AbilityType.EmployeeAmount:
                    return 0;
                default:
                    Debug.LogWarning($"failed get default ability : {abilityType}");
                    return 0;
            }
        }
    }
}