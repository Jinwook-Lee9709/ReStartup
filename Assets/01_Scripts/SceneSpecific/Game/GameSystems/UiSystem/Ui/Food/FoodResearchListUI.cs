using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using Excellcube.EasyTutorial.Utils;
using UnityEngine;
using UnityEngine.UI;
using static SoonsoonData;

public class FoodResearchListUI : MonoBehaviour
{
    public GameObject researchItemObject;
    private FoodData data;
    private Dictionary<int, FoodResearchUIItem> foodResearchItems = new();

    void Start()
    {
        var userDataManager = UserDataManager.Instance;
        userDataManager.ChangeRankPointAction += Unlock;
        if (UserDataManager.Instance.CurrentUserData.CurrentRankPoint != null)
            Unlock((int)UserDataManager.Instance.CurrentUserData.CurrentRankPoint);
        ServiceLocator.Instance.GetSceneService<GameManager>().WorkFlowController.OnCookingStationAdded += UnlockCheakCookwareType;
        float newWidth = Screen.width * 0.3f;
        float newHeight = Screen.width * 0.2f;
        gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(newWidth, newHeight);
        UnlockCheakAll();
    }
    private void UnlockCheakAll()
    {
        foreach (var ui in foodResearchItems)
        {
            ui.Value.UnlockCookwareAmount();
            ui.Value.UnlockFood();
        }
    }
    public void AddFoodResearchItem(FoodData data, FoodResearchNotifyPopup notifyPopup)
    {
        var ui = Instantiate(researchItemObject, transform).GetComponent<FoodResearchUIItem>();
        ui.Init(data, notifyPopup);

        if(ui.foodData.FoodID == 301001)
        {
            ui.gameObject.AddComponent<TutorialSelectionTarget>().Key = "FoodUnLockTutorial";
        }

        foodResearchItems.Add(data.Requirements, ui);
    }
    public void Unlock(int currentRankingPoint)
    {
        foreach (var ui in foodResearchItems)
        {
            ui.Value.UnlockFood();
        }
    }
    public void UnlockCheakCookwareType(CookwareType type)
    {
        foreach (var ui in foodResearchItems)
        {
            if (ui.Value.foodData.CookwareType == type)
            {
                ui.Value.UnlockCookwareAmount();
                ui.Value.UnlockFood();
            }
        }
    }
}
