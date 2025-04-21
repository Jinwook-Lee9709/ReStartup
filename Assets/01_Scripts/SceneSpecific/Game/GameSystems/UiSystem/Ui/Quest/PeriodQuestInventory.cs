using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PeriodQuestInventory : MonoBehaviour
{
    public AssetReference questCard;

    public Transform dailyQuestContents;
    public Transform weeklyQuestContents;

    private List<QuestCard> dalilQuestList = new();
    private List<QuestCard> weeklyQuestList = new();

    [SerializeField] public GameObject dailyQuestScrollview, weeklyQuestScrollview, mainQuestScrollview, AchievementScrollview;

    public void AddPeriodQuest(PeriodQuestData data)
    {
        GameObject newCard = null;
        switch (data.QuestType)
        {
            case QuestType.Daily:
                newCard = Addressables.InstantiateAsync(questCard, dailyQuestContents).WaitForCompletion();
                var newDailyCard = newCard.GetComponent<QuestCard>();
                newDailyCard.Init(data);
                dalilQuestList.Add(newDailyCard);
                break;
            case QuestType.Weekly:
                newCard = Addressables.InstantiateAsync(questCard, weeklyQuestContents).WaitForCompletion();
                var newWeeklyCard = newCard.GetComponent<QuestCard>();
                newWeeklyCard.Init(data);
                weeklyQuestList.Add(newWeeklyCard);
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
