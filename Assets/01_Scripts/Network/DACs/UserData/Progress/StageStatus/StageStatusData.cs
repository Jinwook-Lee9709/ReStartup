using System;
using Newtonsoft.Json;
public class StageStatusData
{
    [JsonConverter(typeof(ThemeIdConverter))]
    public ThemeIds theme;
    [JsonProperty("is_cleared")]
    public bool isCleared;
    [JsonProperty("last_claim")]
    public DateTime lastClaim;
    [JsonProperty("manager_count")]
    public int managerCount;
}

public class ThemeIdConverter : JsonConverter<ThemeIds>
{
    public override void WriteJson(JsonWriter writer, ThemeIds value, JsonSerializer serializer)
    {
        writer.WriteValue((int)value);
    }
    public override ThemeIds ReadJson(JsonReader reader, Type objectType, ThemeIds existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return (ThemeIds)(int)(long)reader.Value;
    }
}
