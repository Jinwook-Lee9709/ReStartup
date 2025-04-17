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
        //switch (periodQuestData.RequirementsType)//미션 종류에 따라 액션 구독해주기 횟수 올려주기 위해
        //{
        //    case RequirementsType.SoldFood:
        //        break;
        //    case RequirementsType.MakingFood:
        //        break;
        //}
    }
    public void OnButtonClick()
    {
        if (periodQuestData.completionsCount >= periodQuestData.RequirementsValue)
        {
            return;
        }
        button.interactable = false;
        CompleteImage.SetActive(true);
        switch (periodQuestData.RewardType1)
        {
            case CostType.Free:
                break;
            case CostType.Money:
                UserDataManager.Instance.CurrentUserData.Money += periodQuestData.RewardAmount1;
                break;
            case CostType.Gold:
                break;
        }
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
