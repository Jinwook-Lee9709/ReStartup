using UnityEngine;

public class FoodScrollView : MonoBehaviour
{
    [SerializeField] private GameObject upgradeViews;
    [SerializeField] private GameObject ResearchViews;
    [SerializeField] private FoodUpgradeListUI foodUpgradeListUI;
    [SerializeField] private FoodResearchListUI foodResearchListUI;
    [SerializeField] private FoodResearchNotifyPopup authorityNotifyPopup;
    [SerializeField] private FoodResearchPopup popup;

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
        foodResearchListUI.AddFoodResearchItem(data, authorityNotifyPopup, popup);
        foodUpgradeListUI.AddFoodUpgradeItem(data);
    }
    public void UnlockFoodUpgrade(FoodData data)
    {
        foodUpgradeListUI.UnlockFood(data);
    }
}
