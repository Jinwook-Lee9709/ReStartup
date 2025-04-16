using System;
using Newtonsoft.Json;
using System;

[Serializable]
public class PromotionData
{
    public int id;
    [JsonProperty("buy_use_count")]
    public int buyUseCount;
    [JsonProperty("ad_use_count")]
    public int adUseCount;
}