using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class MissionManager
{
    private PeriodQuestInventory questInventory;
    private GameManager gameManager;
    private Dictionary<MissionMainCategory, List<Mission>> missions;

    public event Action<int> eve;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
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
            
        }
    }
    public void OnMissonCleared(Mission mission)
    {

    }
    public void OnEventInvoked(MissionMainCategory category, int id)
    {
        foreach (var mission in missions[category])
        {
            if (mission.OnEventInvoked(id))
            {
                OnMissonCleared(mission);
            }
        }
    }
    public void SubscribeMissionTarget(MissionData card)
    {
        switch (card.MainCategory)
        {
            case MissionMainCategory.BuyInterior:
                break;
            case MissionMainCategory.UnlockFood:
                break;
            case MissionMainCategory.SellingFood:
                break;
            case MissionMainCategory.UpgradeFood:
                break;
            case MissionMainCategory.HireStaff:
                break;
            case MissionMainCategory.Promotion:
                break;
            case MissionMainCategory.UpgradeInterior:
                break;
            case MissionMainCategory.GuestSatisfied:
                break;
            case MissionMainCategory.UpgradeStaff:
                break;
            case MissionMainCategory.Clean:
                break;
            case MissionMainCategory.Recover:
                break;
            case MissionMainCategory.GainMoney:
                break;
            case MissionMainCategory.Guest:
                break;
        }
    }
}