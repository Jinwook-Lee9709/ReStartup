using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodQuestData
{
    public int PeriodQuestID { get; set; }
    public QuestType QuestType { get; set; }
    public RequirementsType RequirementsType { get; set; }
    public int RequirementsValue { get; set; }
    public string RewardType1 { get; set; }
    public int RewardAmount1 { get; set; }
    public int Theme { get; set; }

    public int completionsCount;
}
