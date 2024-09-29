using System;
using System.Collections.Generic;

[Serializable]
public class RestaurantData
{
    public int unlockableObjectCount;
    public int money;
    public int paidAmount;
    public Dictionary<int, int> abilityDataDict = new ();
}
