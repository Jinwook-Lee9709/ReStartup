using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public PeriodQuestInventory questInventory;
    void Start()
    {
        var gameManager = ServiceLocator.Instance.GetComponent<GameManager>();
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
