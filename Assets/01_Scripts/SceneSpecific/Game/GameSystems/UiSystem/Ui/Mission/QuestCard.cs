using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector;

public class QuestCard : MonoBehaviour
{
    //[SerializeField] private GameObject CompleteImage;
    private readonly string categoryFormat = "MainCategory{0}";
    private readonly string missionName = "MissionName{0}";
    private readonly string completeGetReward = "CompleteGetReward";
    private readonly string nonComplete = "NonComplete";
    private readonly string rewardReceipt = "RewardReceipt";
    [SerializeField] private TextMeshProUGUI rewardValue, conditionText, currentProgress, buttonText;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Image rewardImage, completeImage;
    public bool clear;
    public bool rewardClaimed;
    public MissionData missionData;
    private MissionManager missionManager;
    private Button button;
    private Mission mission;

    public void Init(MissionData data, Mission mis, bool saveClear)
    {
        missionData = data;
        mission = mis;
        button = GetComponentInChildren<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClick);
        button.interactable = mission.Count >= missionData.CompleteTimes;
        missionManager = ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager;
        rewardValue.text = $"X {missionData.RewardAmount}";
        progressSlider.value = (float)mission.Count / missionData.CompleteTimes;
        conditionText.text = string.Format(LZString.GetUIString(string.Format(missionName, missionData.MissionId)), missionData.CompleteTimes);
        currentProgress.text = $"{Math.Clamp(mission.Count, 0, missionData.CompleteTimes)} / {missionData.CompleteTimes}";
        button.GetComponentInChildren<TextMeshProUGUI>().text = mission.Count < missionData.CompleteTimes ? LZString.GetUIString(nonComplete) : LZString.GetUIString(completeGetReward);
        clear = mission.Count >= missionData.CompleteTimes;
        rewardClaimed = false;
        if (saveClear && missionData.MissionType != MissionType.Achievements)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = LZString.GetUIString(rewardReceipt);
            rewardClaimed = true;
            completeImage.gameObject.SetActive(true);
        }
        missionManager.ReorderMissionCard(missionData.MissionId);
    }
    public void Init(MissionData data, Mission mis)
    {
        missionData = data;
        mission = mis;
        button = GetComponentInChildren<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnButtonClick);
        button.interactable = mission.Count >= missionData.CompleteTimes;
        missionManager = ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager;
        rewardValue.text = $"X {missionData.RewardAmount}";
        progressSlider.value = (float)mission.Count / missionData.CompleteTimes;
        conditionText.text = string.Format(LZString.GetUIString(string.Format(missionName, missionData.MissionId)), missionData.CompleteTimes);
        currentProgress.text = $"{Math.Clamp(mission.Count, 0, missionData.CompleteTimes)} / {missionData.CompleteTimes}";
        button.GetComponentInChildren<TextMeshProUGUI>().text = mission.Count < missionData.CompleteTimes ? LZString.GetUIString(nonComplete) : LZString.GetUIString(completeGetReward);
        clear = mission.Count >= missionData.CompleteTimes;
        rewardClaimed = false;
        missionManager.ReorderMissionCard(missionData.MissionId);
    }
    public void OnButtonClick()
    {
        if(!clear)
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
                UserDataManager.Instance.AdjustGold(missionData.RewardAmount);
                break;
            case RewardType.AdBlockTicket:
                break;
            case RewardType.MissionPoint:
                break;
            case RewardType.RankPoint:
                UserDataManager.Instance.AddRankPointWithSave(missionData.RewardAmount).Forget();
                break;
        }
        if (missionData.MissionType == MissionType.Achievements)
        {
            if (missionData.NextMissionId != 0)
            {
                missionManager.OnMissionCleared(missionData, mission);
                rewardClaimed = true;
                return;
            }
        }

        missionManager.OnMissionCleared(missionData, mission);
        rewardClaimed = true;
        completeImage.gameObject.SetActive(true);
        missionManager.ReorderMissionCard(missionData.MissionId);
        button.GetComponentInChildren<TextMeshProUGUI>().text = LZString.GetUIString(rewardReceipt);
    }
    public void UpdateMissionUICard(int count)
    {
        currentProgress.text = $"{Math.Clamp(count, 0, missionData.CompleteTimes)} / {missionData.CompleteTimes}";
        progressSlider.value = (float)count / missionData.CompleteTimes;
        clear = count >= missionData.CompleteTimes;
        if (clear)
        {
            ServiceLocator.Instance.GetSceneService<GameManager>().uiManager.OnMissionClear();
            missionManager.OnQuestClear(missionData.MissionType);
            button.GetComponentInChildren<TextMeshProUGUI>().text = LZString.GetUIString(completeGetReward);
            button.interactable = true;
            missionManager.ReorderMissionCard(missionData.MissionId);
        }
    }

    public void ResetQuest()
    {
        //퀘스트 초기화 해주기
    }
}
