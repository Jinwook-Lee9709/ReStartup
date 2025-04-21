using System;
using Newtonsoft.Json;

public class BuffSaveData
{
    public int id;
    [JsonProperty("buff_type")][JsonConverter(typeof(BuffTypeConverter))]
    public BuffType type;
    [JsonProperty("remain_time")]
    public float remainTime;
}

public class BuffTypeConverter : JsonConverter<BuffType>
{
    public override void WriteJson(JsonWriter writer, BuffType value, JsonSerializer serializer)
    {
        // Enum -> String 변환
        writer.WriteValue(value.ToString().ToLower());
    }

    public override BuffType ReadJson(JsonReader reader, Type objectType, BuffType existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        // String -> Enum 변환
        var value = reader.Value?.ToString()?.ToLower();

        return value switch
        {
            "FootTraffic" => BuffType.FootTraffic,
            "StaffWork" => BuffType.StaffWork,
            "StaffMove" => BuffType.StaffMove,
            "PairSpawn" => BuffType.PairSpawn,
            "TimerSpeed" => BuffType.TimerSpeed,
            _ => throw new JsonSerializationException($"Unexpected currency_type value: {value}")
        };
    }
}
