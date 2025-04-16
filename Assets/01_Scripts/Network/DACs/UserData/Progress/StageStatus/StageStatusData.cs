using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class StageStatusData
{
    [JsonConverter(typeof(ThemeIdConverter))]
    public ThemeIds theme;
    [JsonProperty("is_cleared")]
    public bool isCleared;
    [JsonProperty("last_Played")]
    public DateTime lastPlayed;
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
