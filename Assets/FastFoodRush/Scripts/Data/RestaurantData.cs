using System;
using System.Collections.Generic;

namespace FastFoodRush.Scripts.Data
{
    [Serializable]
    public class AbilityData
    {
        public int level;
        public float statusValue;
    }

    [Serializable]
    public class RestaurantData
    {
        public int unlockableObjectCount;
        public int money;
        public int paidAmount;
        public bool isUnlock;
        public Dictionary<int, AbilityData> abilityLevelDataDict = new ();
    }
}