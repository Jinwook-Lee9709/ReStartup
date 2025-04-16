using Newtonsoft.Json;

public class BuffSaveData
{
    public int id;
    [JsonProperty("remain_time")]
    public float remainTime;
}
