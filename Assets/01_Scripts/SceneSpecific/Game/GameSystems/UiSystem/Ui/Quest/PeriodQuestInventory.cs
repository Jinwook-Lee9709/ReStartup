using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PeriodQuestInventory : MonoBehaviour
{
    public AssetReference dailyQuest;
    public AssetReference weeklyQuest;

    public Transform dailyQuestContents;
    public Transform weeklyQuestContents;

    private List<QuestCard> dalilQuestList;
    private List<QuestCard> weeklyQuestList;

    public void AddPeriodQuest(PeriodQuestData data)
    {
        GameObject newCard = null;
        switch (data.QuestType)
        {
            case QuestType.Daily:
                newCard = Addressables.InstantiateAsync(dailyQuest, dailyQuestContents).WaitForCompletion();
                var newDailyCard = newCard.GetComponent<QuestCard>();
                newDailyCard.Init(data);
                dalilQuestList.Add(newDailyCard);
                break;
            case QuestType.Weekly:
                newCard = Addressables.InstantiateAsync(weeklyQuest, weeklyQuestContents).WaitForCompletion();
                var newWeeklyCard = newCard.GetComponent<QuestCard>();
                newWeeklyCard.Init(data);
                weeklyQuestList.Add(newWeeklyCard);
                break;
        }
    }
}
