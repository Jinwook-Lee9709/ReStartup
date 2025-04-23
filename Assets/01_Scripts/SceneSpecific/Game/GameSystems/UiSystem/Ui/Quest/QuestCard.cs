using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestCard : MonoBehaviour
{
    //[SerializeField] private GameObject CompleteImage;
    private readonly string categoryFormat = "MainCategory{0}";
    private readonly string missionName = "MissionName{0}";
    [SerializeField] private TextMeshProUGUI rewardType, rewardValue, conditionText, currentProgress, buttonText;
    [SerializeField] private Image rewardImage;
    public MissionData missionData;
    private Button button;

    public void Init(MissionData data)
    {
        missionData = data;
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(OnButtonClick);
        button.interactable = false;
        var MissionManager = ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager;
        rewardValue.text = missionData.RewardAmount.ToString();
        rewardType.text = missionData.RewardType.ToString();
        conditionText.text = LZString.GetUIString(string.Format(missionName, missionData.MissionId));
        currentProgress.text = $"{0} / {missionData.CompleteTimes}";
        //currentProgress.text <- 현재 진행 상황 로드해주기
    }
    public void OnButtonClick()
    {
        button.interactable = false;
        //CompleteImage.SetActive(true);
        switch (missionData.RewardType)
        {
            case RewardType.Money:
                UserDataManager.Instance.AdjustMoney(missionData.RewardAmount);
                break;
            case RewardType.Gold:
                UserDataManager.Instance.AdjustGold(missionData.RewardAmount);
                break;
            case RewardType.AdBlockTicket:
                break;
            case RewardType.MissionPoint:
                break;
            case RewardType.RankPoint:
                UserDataManager.Instance.OnRankPointUp(missionData.RewardAmount);
                break;
        }
        Destroy(gameObject);
        //보상지급
    }
    public void UpdateMissionUICard(int count)
    {
        currentProgress.text = count.ToString();
        if (count >= missionData.CompleteTimes)
        {
            button.interactable = true;
        }
    }
    public void ResetQuest()
    {
        //퀘스트 초기화 해주기
    }
}
