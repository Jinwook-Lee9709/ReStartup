using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class EmployeeSaveData
{
    public int id;
    [JsonConverter(typeof(ThemeIdConverter))]
    public ThemeIds theme;
    public int level;
    [JsonProperty("remain_hp")]
    public int remainHp;
    [JsonProperty("remain_hp_decrease_time")]
    public float remainHpDecreaseTime;
}
