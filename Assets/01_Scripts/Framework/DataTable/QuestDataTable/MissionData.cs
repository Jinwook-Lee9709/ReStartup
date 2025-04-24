using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionData
{
    public int MissionId { get; set; }
    public MissionType MissionType { get; set; }
    public int Theme { get; set; }
    public MissionMainCategory MissionCategory { get; set; }
    public int TargetId { get; set; }
    public int CompleteTimes { get; set; }
    public RewardType RewardType { get; set; }
    public int RewardAmount { get; set; }
    public string Icon { get; set; }
}
