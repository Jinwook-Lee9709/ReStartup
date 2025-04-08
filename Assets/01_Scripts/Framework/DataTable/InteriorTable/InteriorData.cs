using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteriorData
{
    public string Name { get; set; }
    public int InteriorID { get; set; }
    public int RestaurantType { get; set; }
    public ObjectArea CookwareType { get; set; }
    public int SellingCost { get; set; }
    public int Requirements1 { get; set; }
    public int Requirements2 { get; set; }
    public string Reward { get; set; }
    public InteriorEffectType EffectType { get; set; }
    public int EffectQuantity { get; set; }
    public int StringID { get; set; }
    public string IconID { get; set; }
}
