using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class MissionManager
{
    private PeriodQuestInventory questInventory;
    private GameManager gameManager;

    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
    public void Start()
    {
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
    public void SubscribeMissionTarget(MisionMainCategory misionMainCategory, MissionData card)
    {
        switch (misionMainCategory)
        {
            case MisionMainCategory.BuyInterior:
                break;
            case MisionMainCategory.UnlockFood:
                break;
            case MisionMainCategory.SellingFood:
                break;
            case MisionMainCategory.UpgradeFood:
                break;
            case MisionMainCategory.HireStaff:
                break;
            case MisionMainCategory.Promotion:
                break;
            case MisionMainCategory.UpgradeInterior:
                break;
            case MisionMainCategory.GuestSatisfied:
                break;
            case MisionMainCategory.UpgradeStaff:
                break;
            case MisionMainCategory.Clean:
                break;
            case MisionMainCategory.Recover:
                break;
            case MisionMainCategory.GainMoney:
                break;
            case MisionMainCategory.Guest:
                break;
        }
    }
}