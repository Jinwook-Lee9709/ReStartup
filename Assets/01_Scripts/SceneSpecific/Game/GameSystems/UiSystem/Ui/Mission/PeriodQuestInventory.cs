using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class PeriodQuestInventory : MonoBehaviour
{
    public AssetReference questCard;

    public Transform dailyMissionContents, weeklyMissionContents, MainMissionContents, achievementsMissionContents;

    private List<QuestCard> dailyQuestList = new();
    private List<QuestCard> weeklyQuestList = new();
    private List<QuestCard> mainQuestList = new();
    private List<QuestCard> rastorantQuestList = new();

    private MissionManager missionManager;

    [SerializeField] private Button daliyButton;
    [SerializeField] private Button weeklyButton;
    [SerializeField] private Button mainQuestButton;
    [SerializeField] private Button rastorantButton;


    [SerializeField] private GameObject dailyQuestScrollview, weeklyQuestScrollview, mainQuestScrollview, AchievementScrollview;

    [SerializeField] private GameObject dailyQuestTap, weeklyQuestTap, mainQuestTap, AchievementTap;

    [SerializeField] private GameObject dailyQuestClearMissionImage, weeklyQuestClearMissionImage, mainQuestClearMissionImage, achievementClearMissionImage;

    [SerializeField] private Button daliyGetAllButton, weeklyGetAllButton, mainQuestGetAllButton, rastorantGetAllButton;
    private void Start()
    {
        daliyButton.onClick.AddListener(OnButtonDaily);
        weeklyButton.onClick.AddListener(OnButtonWeekly);
        mainQuestButton.onClick.AddListener(OnButtonMain);
        rastorantButton.onClick.AddListener(OnButtonAchievement);
        daliyGetAllButton.onClick.AddListener(DailyAllGetButtonClick);
        weeklyGetAllButton.onClick.AddListener(WeeklyAllGetButtonClick);
        mainQuestGetAllButton.onClick.AddListener(MainAllGetButtonClick);
        rastorantGetAllButton.onClick.AddListener(RastorantAllGetButtonClick);
    }

    private void OnEnable()
    {
        OnButtonDaily();
    }
    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                ServiceLocator.Instance.GetSceneService<GameManager>().uiManager.OnClickButtonExitQuestUI();
            }
        }
    }


    public void AddPeriodQuest(MissionData data, Mission mission, bool saveClear)
    {
        if (missionManager == null)
        {
            missionManager = ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager;
        }
        GameObject newCard = null;
        switch (data.MissionType)
        {
            case MissionType.Main:
                newCard = Addressables.InstantiateAsync(questCard, MainMissionContents).WaitForCompletion();
                var newMainCard = newCard.GetComponent<QuestCard>();
                newMainCard.Init(data, mission, saveClear);
                mainQuestList.Add(newMainCard);
                missionManager.missionCards.Add(data.MissionId, newMainCard);
                break;
            case MissionType.Daily:
                newCard = Addressables.InstantiateAsync(questCard, dailyMissionContents).WaitForCompletion();
                var newDailyCard = newCard.GetComponent<QuestCard>();
                newDailyCard.Init(data, mission, saveClear);
                dailyQuestList.Add(newDailyCard);
                missionManager.missionCards.Add(data.MissionId, newDailyCard);
                break;
            case MissionType.Weekly:
                newCard = Addressables.InstantiateAsync(questCard, weeklyMissionContents).WaitForCompletion();
                var newWeeklyCard = newCard.GetComponent<QuestCard>();
                newWeeklyCard.Init(data, mission, saveClear);
                weeklyQuestList.Add(newWeeklyCard);
                missionManager.missionCards.Add(data.MissionId, newWeeklyCard);
                break;
            case MissionType.Achievements:
                newCard = Addressables.InstantiateAsync(questCard, achievementsMissionContents).WaitForCompletion();
                var newAchievementsCard = newCard.GetComponent<QuestCard>();
                newAchievementsCard.Init(data, mission, saveClear);
                rastorantQuestList.Add(newAchievementsCard);
                missionManager.missionCards.Add(data.MissionId, newAchievementsCard);
                break;
        }
    }
    public void AddPeriodQuest(MissionData data, Mission mission)
    {
        if (missionManager == null)
        {
            missionManager = ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager;
        }
        GameObject newCard = null;
        switch (data.MissionType)
        {
            case MissionType.Main:
                newCard = Addressables.InstantiateAsync(questCard, MainMissionContents).WaitForCompletion();
                var newMainCard = newCard.GetComponent<QuestCard>();
                newMainCard.Init(data, mission);
                mainQuestList.Add(newMainCard);
                missionManager.missionCards.Add(data.MissionId, newMainCard);
                break;
            case MissionType.Daily:
                newCard = Addressables.InstantiateAsync(questCard, dailyMissionContents).WaitForCompletion();
                var newDailyCard = newCard.GetComponent<QuestCard>();
                newDailyCard.Init(data, mission);
                dailyQuestList.Add(newDailyCard);
                missionManager.missionCards.Add(data.MissionId, newDailyCard);
                break;
            case MissionType.Weekly:
                newCard = Addressables.InstantiateAsync(questCard, weeklyMissionContents).WaitForCompletion();
                var newWeeklyCard = newCard.GetComponent<QuestCard>();
                newWeeklyCard.Init(data, mission);
                weeklyQuestList.Add(newWeeklyCard);
                missionManager.missionCards.Add(data.MissionId, newWeeklyCard);
                break;
            case MissionType.Achievements:
                newCard = Addressables.InstantiateAsync(questCard, achievementsMissionContents).WaitForCompletion();
                var newAchievementsCard = newCard.GetComponent<QuestCard>();
                newAchievementsCard.Init(data, mission);
                rastorantQuestList.Add(newAchievementsCard);
                missionManager.missionCards.Add(data.MissionId, newAchievementsCard);
                break;
        }
    }
    private void DailyAllGetButtonClick()
    {
        foreach (var item in dailyQuestList)
        {
            if (!item.rewardClaimed && item.clear)
            {
                item.OnButtonClick();
            }
        }
    }
    private void WeeklyAllGetButtonClick()
    {
        foreach (var item in weeklyQuestList)
        {
            if (!item.rewardClaimed && item.clear)
            {
                item.OnButtonClick();
            }
        }
    }
    private void MainAllGetButtonClick()
    {
        foreach (var item in mainQuestList)
        {
            if (!item.rewardClaimed && item.clear)
            {
                item.OnButtonClick();
            }
        }
    }
    private void RastorantAllGetButtonClick()
    {
        foreach (var item in rastorantQuestList)
        {
            if (item.clear)
            {
                item.OnButtonClick();
            }
        }
    }
    public void OnButtonDaily()
    {
        dailyQuestClearMissionImage.gameObject.SetActive(false);
        daliyButton.interactable = false;
        weeklyButton.interactable = true;
        mainQuestButton.interactable = true;
        rastorantButton.interactable = true;
        dailyQuestTap.SetActive(true);
        weeklyQuestTap.SetActive(false);
        mainQuestTap.SetActive(false);
        AchievementTap.SetActive(false);

    }
    public void OnButtonWeekly()
    {
        weeklyQuestClearMissionImage.gameObject.SetActive(false);
        daliyButton.interactable = true;
        weeklyButton.interactable = false;
        mainQuestButton.interactable = true;
        rastorantButton.interactable = true;
        dailyQuestTap.SetActive(false);
        weeklyQuestTap.SetActive(true);
        mainQuestTap.SetActive(false);
        AchievementTap.SetActive(false);
    }
    public void OnButtonMain()
    {
        mainQuestClearMissionImage.gameObject.SetActive(false);
        daliyButton.interactable = true;
        weeklyButton.interactable = true;
        mainQuestButton.interactable = false;
        rastorantButton.interactable = true;
        dailyQuestTap.SetActive(false);
        weeklyQuestTap.SetActive(false);
        mainQuestTap.SetActive(true);
        AchievementTap.SetActive(false);
    }
    public void OnButtonAchievement()
    {
        achievementClearMissionImage.gameObject.SetActive(false);
        daliyButton.interactable = true;
        weeklyButton.interactable = true;
        mainQuestButton.interactable = true;
        rastorantButton.interactable = false;
        dailyQuestTap.SetActive(false);
        weeklyQuestTap.SetActive(false);
        mainQuestTap.SetActive(false);
        AchievementTap.SetActive(true);
    }
    public void newMissionClear(MissionType type)
    {
        switch (type)
        {
            case MissionType.Achievements:
                achievementClearMissionImage.SetActive(true);
                break;
            case MissionType.Main:
                mainQuestClearMissionImage.SetActive(true);
                break;
            case MissionType.Daily:
                dailyQuestClearMissionImage.SetActive(true);
                break;
            case MissionType.Weekly:
                weeklyQuestClearMissionImage.SetActive(true);
                break;
        }
    }
    public void ResetDailyQuest()
    {
        foreach (var quest in dailyQuestList)
        {
            quest.ResetQuest();
        }
    }
    public void ResetWeeklyQuest()
    {
        foreach (var quest in weeklyQuestList)
        {
            quest.ResetQuest();
        }
    }

}
