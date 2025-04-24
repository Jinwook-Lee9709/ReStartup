using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class RankerData
{
    public string uuid;
    public string name;
    [JsonProperty("total_rank_point")] public int rankPoint;
    
}

public class UserRankData
{
    [JsonProperty("ranking")]
    public int rank;
    [JsonProperty("total_rank_point")]
    public int rankPoint;
}
