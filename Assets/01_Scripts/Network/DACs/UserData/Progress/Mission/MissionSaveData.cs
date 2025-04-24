using Newtonsoft.Json;

public class MissionSaveData
{
    [JsonProperty("mission_id")]
    public int id;
    public int count;
    [JsonProperty("is_cleared")]
    public bool isCleared;
}
