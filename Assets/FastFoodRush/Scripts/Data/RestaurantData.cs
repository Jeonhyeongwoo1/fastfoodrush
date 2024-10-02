using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

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
    public int restaurantId;
    public bool isUnlock;
    public Dictionary<int, AbilityData> abilityLevelDataDict = new ();
}
