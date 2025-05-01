using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankConditionData
{
    public int RangkingID { get; set; }
    public int Type { get; set; }
    public int Rank { get; set; }
    public int GoalRanking { get; set; }
    public RankRewardType RewardType1 { get; set; }
    public int RewardAmount1 { get; set; }
    public RankRewardType RewardType2 { get; set; }
    public int RewardAmount2 { get; set; }
    public int StringID { get; set; }
    public string IconName { get; set; }
}
