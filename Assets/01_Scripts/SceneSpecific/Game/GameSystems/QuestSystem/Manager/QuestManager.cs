using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class QuestManager
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
        var questdata = DataTableManager.Get<PeriodQuestDataTable>("periodQuest").Data;
        foreach (var item in questdata.Values)
        {
            if (item.Theme == (int)gameManager.CurrentTheme)
            {
                questInventory.AddPeriodQuest(item);
            }
        }
    }
}
