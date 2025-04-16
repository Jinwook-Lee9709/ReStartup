using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestCard : MonoBehaviour
{
    [SerializeField] private GameObject CompleteImage; 
    private PeriodQuestData periodQuestData;
    private Button button;

    public void Init(PeriodQuestData data)
    {
        periodQuestData = data;
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(OnButtonClick);
        CompleteImage.SetActive(true);
    }
    public void OnButtonClick()
    {
        if (periodQuestData.completionsCount >= periodQuestData.RequirementsValue)
        {
            return;
        }
        button.interactable = false;
        CompleteImage.SetActive(true);
        //보상지급
    }
    public void UpCount()
    {
        if (periodQuestData.completionsCount >= periodQuestData.RequirementsValue)
        {
            return;
        }
        ++periodQuestData.completionsCount;
    }
    public void ResetQuest()
    {
        periodQuestData.completionsCount = 0;
    }
}
