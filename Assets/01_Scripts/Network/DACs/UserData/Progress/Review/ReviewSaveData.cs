using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ReviewSaveData
{
    [JsonProperty("order_index")]
    public int orderIndex;
    [JsonProperty("is_positive")]
    public bool isPositive;
    [JsonProperty("review_id")]
    public int reviewId;

    [JsonProperty("created_time")] public DateTime createdTime;
}
