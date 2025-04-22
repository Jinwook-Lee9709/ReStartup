using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestCard : MonoBehaviour
{
    //[SerializeField] private GameObject CompleteImage;
    [SerializeField] private TextMeshProUGUI rewardType, rewardValue, conditionText, currentProgress, buttonText;
    [SerializeField] private Image rewardImage;
    private MissionData missionData;
    private Button button;

    public void Init(MissionData data)
    {
        missionData = data;
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(OnButtonClick);
        var MissionManager = ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager;
        MissionManager.SubscribeMissionTarget(missionData.MainCategory, missionData);
    }
    public void OnButtonClick()
    {
        if (missionData.count >= missionData.CompleteTimes)
        {
            return;
        }
        button.interactable = false;
        //CompleteImage.SetActive(true);
        switch (missionData.RewardType)
        {
            case RewardType.Money:
                UserDataManager.Instance.AdjustMoney(missionData.RewardAmount);
                break;
            case RewardType.Gold:
                break;
            case RewardType.AdBlockTicket:
                break;
            case RewardType.MissionPoint:
                break;
            case RewardType.RankPoint:
                break;
        }
        //보상지급
    }
    public void UpCount()
    {
        if (missionData.count >= missionData.CompleteTimes)
        {
            return;
        }
        ++missionData.count;
    }
    public void ResetQuest()
    {
        missionData.count = 0;
    }
}
