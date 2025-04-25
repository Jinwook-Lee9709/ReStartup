using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
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
    public bool clear;
    public MissionData missionData;
    private MissionManager missionManager;
    private Button button;

    public void Init(MissionData data, Mission mission)
    {
        missionData = data;
        button = GetComponentInChildren<Button>();
        button.onClick.AddListener(OnButtonClick);
        button.interactable = mission.Count >= missionData.CompleteTimes;
        missionManager = ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager;
        rewardValue.text = $"X {missionData.RewardAmount}";
        progressSlider.value = (float)mission.Count / missionData.CompleteTimes;
        conditionText.text = string.Format(LZString.GetUIString(string.Format(missionName, missionData.MissionId)), missionData.CompleteTimes);
        currentProgress.text = $"{Math.Clamp(mission.Count, 0, missionData.CompleteTimes)} / {missionData.CompleteTimes}";
        button.GetComponentInChildren<TextMeshProUGUI>().text = mission.Count < missionData.CompleteTimes ? "미완료" : "완료 \n 보상 받기";
        clear = mission.Count >= missionData.CompleteTimes;
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

        missionManager.OnMissionCleared(missionData);
        Destroy(gameObject);
        //보상지급
    }
    public void UpdateMissionUICard(int count)
    {
        currentProgress.text = $"{Math.Clamp(count, 0, missionData.CompleteTimes)} / {missionData.CompleteTimes}";
        progressSlider.value = (float)count / missionData.CompleteTimes;
        clear = count >= missionData.CompleteTimes;
        if (clear)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = "완료 \n 보상 받기";
            button.interactable = true;
            missionManager.ReorderMissionCard(missionData.MissionId);
        }
    }
    public void ResetQuest()
    {
        //퀘스트 초기화 해주기
    }
}
