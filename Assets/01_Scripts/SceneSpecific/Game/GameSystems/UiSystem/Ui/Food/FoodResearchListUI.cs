using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.UI;
using static SoonsoonData;

public class FoodResearchListUI : MonoBehaviour
{
    public GameObject researchItemObject;
    private FoodData data;
    private Dictionary<int ,FoodResearchUIItem> foodResearchItems = new();
    void Start()
    {
        var userDataManager = UserDataManager.Instance;
        userDataManager.ChangeRankPointAction += Unlock;
    }
    public void AddFoodResearchItem(FoodData data, FoodResearchNotifyPopup notifyPopup)
    {
        var ui = Instantiate(researchItemObject, transform).GetComponent<FoodResearchUIItem>();
        ui.Init(data, notifyPopup);
        foodResearchItems.Add(data.Requirements ,ui);
    }
    public void Unlock(int currentRankingPoint)
    {
        var currentTheme = ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;
        foreach (var ui in foodResearchItems)
        {
            var cookwareType = ui.Value.foodData.CookwareType;
            int currentCookwareAmount = UserDataManager.Instance.CurrentUserData.CookWareUnlock[currentTheme][cookwareType];
            var chackCookWareUnlock = currentCookwareAmount < ui.Value.foodData.CookwareNB;
            if (ui.Key < currentRankingPoint && !chackCookWareUnlock)
            {
                ui.Value.UnlockFood();
            }
        }
    }
}
