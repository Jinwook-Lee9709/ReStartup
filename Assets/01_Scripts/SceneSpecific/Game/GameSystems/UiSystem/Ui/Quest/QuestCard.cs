using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestCard : MonoBehaviour
{
    private PeriodQuestData periodQuestData;

    public void Init(PeriodQuestData data)
    {
        periodQuestData = data;

    }
    public void UpCount()
    {
        ++periodQuestData.requirmentsCount;
    }
}
