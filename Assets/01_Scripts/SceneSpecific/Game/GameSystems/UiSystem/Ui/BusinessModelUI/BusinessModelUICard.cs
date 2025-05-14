using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.AddressableAssets.GUI;
using UnityEngine;
using UnityEngine.UI;

public class BusinessModelUICard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI eaText;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] Image image;
    [SerializeField] Image costImage;
    public int ea;
    public CostType costType;
    [SerializeField] Button mainButton;
    public RewardType rewardType;
    private BusinessModelUIBuyPopup popup;
    public BusinessModelUI businessModelUI;
    public int cost;

    private void Start()
    {
        businessModelUI = ServiceLocator.Instance.GetSceneService<GameManager>().uiManager.uiBusinessModel.GetComponent<BusinessModelUI>();
        popup = businessModelUI.busunessModelUIBuyPopup.GetComponent<BusinessModelUIBuyPopup>();
        eaText.text = $"X {ea}";
        costText.text = cost.ToString("N0");
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(OnPopup);
    }
    private void OnPopup()
    {
        popup.gameObject.SetActive(true);
        popup.SetInfo(this,image.sprite, costImage.sprite);
    }
    private void OnNotEnoughCostPopup()
    {
        businessModelUI.OnNotEnoughCostPopup();
    }
    public async void GoldBuy()
    {
        //팝업켜기 킨후 아래행동
        var userDataManager = UserDataManager.Instance;
        if(userDataManager.CurrentUserData.Gold < cost)
        {
            OnNotEnoughCostPopup();
            return;
        }
        userDataManager.AdjustGold(-cost);
        switch (rewardType)
        {
            case RewardType.Money:
                await userDataManager.AdjustMoneyWithSave(ea);
                break;
            case RewardType.Gold:
                await userDataManager.AdjustGoldWithSave(ea);
                break;
            case RewardType.AdBlockTicket:
                await userDataManager.AdjustAdTicketWithSave(ea);
                break;
        }
    }
    public async void CashBuy()
    {
        switch (rewardType)
        {
            case RewardType.Money:
                await UserDataManager.Instance.AdjustMoneyWithSave(ea);
                break;
            case RewardType.Gold:
                await UserDataManager.Instance.AdjustGoldWithSave(ea);
                break;
            case RewardType.AdBlockTicket:
                await UserDataManager.Instance.AdjustAdTicketWithSave(ea);
                break;
        }
    }
}
