using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using DG.Tweening.Core.Easing;
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
            if(isExist && saveData.isCleared)
                continue;


            if (item.Theme != (int)gameManager.CurrentTheme && item.Theme != 0)
            {
                continue;
            }
            var newMission = new Mission();
            newMission.Init(item.CompleteTimes, item.MissionId, item.TargetId);
            if(isExist)
                newMission.SetCount(saveData.count);
            missions[item.MissionCategory].Add(newMission);
            questInventory.AddPeriodQuest(item, newMission);
        }
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
        var mission = missions[data.MissionCategory].Find(x => x.ID == data.MissionId);
        Mission.SavePrgoress(true, mission).Forget();
    }
    public void RemoveMissionCard(int id)
    {
        missionCards.Remove(id);
    }
    private void UpdateMissionUICard(int args, Mission mission)
    {
        if (missionCards.TryGetValue(mission.ID, out var card))
        {
            card.UpdateMissionUICard(args);
        }
    }
}