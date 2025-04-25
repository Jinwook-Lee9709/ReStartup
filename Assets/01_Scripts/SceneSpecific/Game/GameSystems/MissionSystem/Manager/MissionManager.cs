using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
public class MissionManager
{
    private PeriodQuestInventory questInventory;
    private GameManager gameManager;
    private Dictionary<MissionMainCategory, List<Mission>> missions;

    public event Action<int> eve;
    public Dictionary<int, QuestCard> missionCards = new();

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        InitDictionary();
    }

    private void InitDictionary()
    {
        missions = new Dictionary<MissionMainCategory, List<Mission>>();
        foreach (var item in Enum.GetValues(typeof(MissionMainCategory)))
        {
            missions.Add((MissionMainCategory)item, new List<Mission>());
        }
    }
    public void Start()
    {
        //로드해서 가져옴
        questInventory = GameObject.FindWithTag("UIManager").GetComponent<UiManager>().uiQuest.GetComponent<PeriodQuestInventory>();
        var questdata = DataTableManager.Get<MissionDataTable>(DataTableIds.Mission.ToString()).Data;
        foreach (var item in questdata.Values)
        {
            bool isExist = UserDataManager.Instance.CurrentUserData.MissionSaveData.TryGetValue(item.MissionId, out var saveData);
            if (isExist && saveData.isCleared)
                continue;


            if (item.Theme != (int)gameManager.CurrentTheme && item.Theme != 0)
            {
                continue;
            }
            var newMission = new Mission();
            newMission.Init(item.CompleteTimes, item.MissionId, item.TargetId);
            if (isExist)
                newMission.SetCount(saveData.count);
            missions[item.MissionCategory].Add(newMission);
            questInventory.AddPeriodQuest(item, newMission);
        }
        AllReorderMissionCard();
    }
    public void OnEventInvoked(MissionMainCategory category, int args, int id = 0)
    {
        foreach (var mission in missions[category])
        {
            mission.OnEventInvoked(args, id);
            UpdateMissionUICard(mission.Count, mission);
        }
    }

    public void OnMissionCleared(MissionData data)
    {
        missionCards.Remove(data.MissionId);
        var mission = missions[data.MissionCategory].Find(x => x.ID == data.MissionId);
        Mission.SavePrgoress(true, mission).Forget();
    }
    public void ReorderMissionCard(int id)
    {
        var data = DataTableManager.Get<MissionDataTable>(DataTableIds.Mission.ToString()).GetMissionData(id);
        var sortedList = missionCards
            .Where(pair => pair.Value.missionData.MissionType == data.MissionType)
            .OrderBy(pair => pair.Value.clear ? 0 : 1)
            .ThenBy(pair => pair.Value.missionData.MissionId)
            .ToList();
        for (int i = 0; i < sortedList.Count; i++)
        {
            var questCard = sortedList[i].Value;

            questCard.transform.SetSiblingIndex(i);
        }
    }
    public void AllReorderMissionCard()
    {
        foreach (MissionType type in Enum.GetValues(typeof(MissionType)))
        {
            var sortedList = missionCards
            .Where(pair => pair.Value.missionData.MissionType == type)
            .OrderBy(pair => pair.Value.clear ? 0 : 1)
            .ThenBy(pair => pair.Value.missionData.MissionId)
            .ToList();

            for (int i = 0; i < sortedList.Count; i++)
            {
                var questCard = sortedList[i].Value;

                questCard.transform.SetSiblingIndex(i);
            }
        }
    }
    private void UpdateMissionUICard(int args, Mission mission)
    {
        if (missionCards.TryGetValue(mission.ID, out var card))
        {
            card.UpdateMissionUICard(args);
        }
    }
}