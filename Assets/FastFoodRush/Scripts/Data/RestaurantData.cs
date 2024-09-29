using System;
using System.Collections.Generic;
using UnityEngine;


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
    
    public Dictionary<int, AbilityData> abilityLevelDataDict = new ();
}
