using Newtonsoft.Json;

public class PromotionData
{
    public int id;
    [JsonProperty("buy_use_count")]
    public int buyUseCount;
    [JsonProperty("ad_use_count")]
    public int adUseCount;
}