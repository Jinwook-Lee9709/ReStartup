using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class FoodSaveData
{
    public int id;
    [JsonConverter(typeof(ThemeIdConverter))]
    public ThemeIds theme;
    public int level;
    [JsonProperty("sell_count")]
    public int sellCount;
}
