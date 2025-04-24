using Newtonsoft.Json;

public class MissionSaveData
{
    public int id;
    public int count;
    [JsonProperty("is_cleared")]
    public bool isCleared;
}
