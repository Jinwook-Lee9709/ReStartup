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
    [SerializeField] private TextMeshProUGUI rewardValue, conditionText, currentProgress, buttonText;
    [SerializeField] private Slider progressSlider;
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
        rewardValue.text = $"X {missionData.RewardAmount}";
        progressSlider.value = 0;
        conditionText.text = string.Format(LZString.GetUIString(string.Format(missionName, missionData.MissionId)), missionData.CompleteTimes);
        currentProgress.text = $"{0} / {missionData.CompleteTimes}";
        button.GetComponentInChildren<TextMeshProUGUI>().text = "미완료";
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
        Debug.Log("구매확인됨");
        currentProgress.text = $"{count} / {missionData.CompleteTimes}";
        progressSlider.value = count / missionData.CompleteTimes;
        if (count >= missionData.CompleteTimes)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = "완료 \n 보상 받기";
            button.interactable = true;
        }
    }
    public void ResetQuest()
    {
        //퀘스트 초기화 해주기
    }
}
