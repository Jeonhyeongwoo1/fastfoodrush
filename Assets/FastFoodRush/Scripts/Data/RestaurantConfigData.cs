using UnityEngine;

namespace FastFoodRush.Scripts.Data
{
    [CreateAssetMenu(fileName = "Restaurant Data", menuName = "Scriptable Object/Restaurant Data")]
    public class RestaurantConfigData : ScriptableObject
    {
        public int MoneyNeedToUnlock => _moneyNeedToUnlock;
        public int PriceOfFood => _priceOfFood;
        
        [SerializeField] private int _moneyNeedToUnlock;
        [SerializeField] private int _priceOfFood;
    }
}