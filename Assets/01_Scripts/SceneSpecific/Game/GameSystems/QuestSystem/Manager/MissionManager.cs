using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class MissionManager
{
    private PeriodQuestInventory questInventory;
    private GameManager gameManager;
    private Dictionary<MissionMainCategory, List<Mission>> missions;

    public event Action<int> eve;
    public List<QuestCard> missionCards = new();

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
            if (item.Theme == (int)gameManager.CurrentTheme || item.Theme == 0)
            {
                questInventory.AddPeriodQuest(item);

            }
            foreach (var mission in missions)
            {
                if (mission.Key == item.MissionCategory)
                {
                    var newMission = new Mission();
                    newMission.Init(item.CompleteTimes, item.MissionId, item.TargetId);
                    mission.Value.Add(newMission);
                }
            }
        }
    }
    public void OnMissonCleared(Mission mission)
    {

    }
    public void OnEventInvoked(MissionMainCategory category, int args = 1, int id = -1)
    {
        foreach (var mission in missions[category])
        {
            if (mission.OnEventInvoked(args, id))
            {
                OnMissonCleared(mission);
            }
            UpdateMissionUICard(args, mission);
        }
    }
    private void UpdateMissionUICard(int args, Mission mission)
    {
        foreach (var uiItem in missionCards)
        {
            if (uiItem.missionData.MissionId == mission.ID)
            {
                uiItem.UpdateMissionUICard(args);
            }
        }
    }
}