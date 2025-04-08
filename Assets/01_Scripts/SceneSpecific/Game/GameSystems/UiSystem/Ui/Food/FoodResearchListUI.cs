using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodResearchListUI : MonoBehaviour
{
    public GameObject researchItemObject;
    public List<Button> allBuyButton;
    void Start()
    {
    }
    public void AddFoodResearchItem(FoodData data)
    {
        var ui = Instantiate(researchItemObject, transform).GetComponent<FoodResearchUIItem>();
        ui.Init(data);
    }
    public void AddButtonList(Button button)
    {
        allBuyButton.Add(button);
    }
    public void FoodAllBuy()
    {
        foreach (var item in allBuyButton)
        {
            item.onClick.Invoke();
        }
    }
}
