using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodResearchListUI : MonoBehaviour
{
    public GameObject researchItemObject;
    public List<Button> allBuyButton;
    private Dictionary<int ,FoodResearchUIItem> foodResearchItems = new();
    void Start()
    {
        var userDataManager = UserDataManager.Instance;
        userDataManager.ChangeRankPointAction += Unlock;
    }
    public void AddFoodResearchItem(FoodData data, FoodResearchNotifyPopup notifyPopup, FoodResearchPopup popup)
    {
        var ui = Instantiate(researchItemObject, transform).GetComponent<FoodResearchUIItem>();
        ui.Init(data, notifyPopup, popup);
        foodResearchItems.Add(data.Requirements ,ui);
    }
    public void AddButtonList(Button button)
    {
        allBuyButton.Add(button);
    }
    public void Unlock(int? currentRankingPoint)
    {
        foreach (var ui in foodResearchItems)
        {
            if(ui.Key < currentRankingPoint)
            {
                ui.Value.UnlockFood();
            }
        }
    }
    public void FoodAllBuy()
    {
        foreach (var item in allBuyButton)
        {
            item.onClick.Invoke();
        }
    }
}
