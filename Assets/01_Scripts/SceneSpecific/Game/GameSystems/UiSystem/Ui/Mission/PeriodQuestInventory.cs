using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PeriodQuestInventory : MonoBehaviour
{
    public AssetReference questCard;

    public Transform dailyMissionContents, weeklyMissionContents, MainMissionContents, achievementsMissionContents;

    private List<QuestCard> dalilQuestList = new();
    private List<QuestCard> weeklyQuestList = new();

    private MissionManager missionManager;


    private void Start()
    {
    }

    [SerializeField] public GameObject dailyQuestScrollview, weeklyQuestScrollview, mainQuestScrollview, AchievementScrollview;

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
                missionManager.missionCards.Add(data.MissionId,newMainCard);
                break;
            case MissionType.Daily:
                newCard = Addressables.InstantiateAsync(questCard, dailyMissionContents).WaitForCompletion();
                var newDailyCard = newCard.GetComponent<QuestCard>();
                newDailyCard.Init(data, mission);
                dalilQuestList.Add(newDailyCard);
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
                missionManager.missionCards.Add(data.MissionId, newAchievementsCard);
                break;
        }
    }
    public void OnButtonDaily()
    {
        dailyQuestScrollview.SetActive(true);
        weeklyQuestScrollview.SetActive(false);
        mainQuestScrollview.SetActive(false);
        AchievementScrollview.SetActive(false);
    }
    public void OnButtonWeekly()
    {
        dailyQuestScrollview.SetActive(false);
        weeklyQuestScrollview.SetActive(true);
        mainQuestScrollview.SetActive(false);
        AchievementScrollview.SetActive(false);
    }
    public void OnButtonMain()
    {
        dailyQuestScrollview.SetActive(false);
        weeklyQuestScrollview.SetActive(false);
        mainQuestScrollview.SetActive(true);
        AchievementScrollview.SetActive(false);
    }
    public void OnButtonAchievement()
    {
        dailyQuestScrollview.SetActive(false);
        weeklyQuestScrollview.SetActive(false);
        mainQuestScrollview.SetActive(false);
        AchievementScrollview.SetActive(true);

    }
    public void ResetDailyQuest()
    {
        foreach (var quest in dalilQuestList)
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
