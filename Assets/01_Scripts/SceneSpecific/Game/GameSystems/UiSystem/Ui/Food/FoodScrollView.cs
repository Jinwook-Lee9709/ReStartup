using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class FoodScrollView : MonoBehaviour
{
    [SerializeField] private GameObject upgradeViews;
    [SerializeField] private GameObject ResearchViews;
    [SerializeField] private FoodUpgradeListUI foodUpgradeListUI;
    [SerializeField] private FoodResearchListUI foodResearchListUI;

    public void SetUpgradeViews()
    {
        upgradeViews.SetActive(true);
        ResearchViews.SetActive(false);
    }
    public void SetResearchViews()
    {
        upgradeViews.SetActive(false);
        ResearchViews.SetActive(true);
    }

    public void AddFoodUISet(FoodData data)
    {
        foodUpgradeListUI.AddFoodUpgradeItem(data);
        foodResearchListUI.AddFoodResearchItem(data);
    }
}
