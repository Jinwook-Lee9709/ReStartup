using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FoodScrollView : MonoBehaviour
{
    [SerializeField] private GameObject upgradeViews;
    [SerializeField] private GameObject ResearchViews;
    [SerializeField] private FoodUpgradeListUI foodUpgradeListUI;
    [SerializeField] private FoodResearchListUI foodResearchListUI;
    [SerializeField] private FoodResearchNotifyPopup authorityNotifyPopup;
    [SerializeField] private FoodUpgradePopup popup;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button researchButton;

    private void Start()
    {
        SetResearchViews();
    }
    
    public void SetUpgradeViews()
    {
        upgradeViews.SetActive(true);
        upgradeButton.interactable = false;
        ResearchViews.SetActive(false);
        researchButton.interactable = true;
    }
    public void SetResearchViews()
    {
        upgradeViews.SetActive(false);
        upgradeButton.interactable = true;
        ResearchViews.SetActive(true); 
        researchButton.interactable = false;
    }

    public void AddFoodUISet(FoodData data)
    {
        foodResearchListUI.AddFoodResearchItem(data, authorityNotifyPopup );
        foodUpgradeListUI.AddFoodUpgradeItem(data, popup);
    }
    public void UnlockFoodUpgrade(FoodData data)
    {
        foodUpgradeListUI.UnlockFood(data);
    }
}
