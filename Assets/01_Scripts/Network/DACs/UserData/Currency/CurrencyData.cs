using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class CurrencyData
{
    [JsonProperty("currency_type")][JsonConverter(typeof(CurrencyTypeConverter))]
    public CurrencyType currencyType; // gold, money 등
    public int amount;   
    
    public CurrencyData(CurrencyType currencyType, int amount)
    {
        this.currencyType = currencyType;
        this.amount = amount;
    }
}


public class CurrencyTypeConverter : JsonConverter<CurrencyType>
{
    public override void WriteJson(JsonWriter writer, CurrencyType value, JsonSerializer serializer)
    {
        // Enum -> String 변환
        writer.WriteValue(value.ToString().ToLower());
    }

    public override CurrencyType ReadJson(JsonReader reader, Type objectType, CurrencyType existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        // String -> Enum 변환
        var value = reader.Value?.ToString()?.ToLower();

        return value switch
        {
            "money" => CurrencyType.Money,
            "gold" => CurrencyType.Gold,
            "adticket" => CurrencyType.AdTicket,
            _ => throw new JsonSerializationException($"Unexpected currency_type value: {value}")
        };
    }
}