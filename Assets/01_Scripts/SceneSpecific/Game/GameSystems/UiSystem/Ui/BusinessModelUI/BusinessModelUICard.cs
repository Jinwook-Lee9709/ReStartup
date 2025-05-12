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
    [SerializeField] int ea;
    [SerializeField] int cost;
    [SerializeField] CostType costType;
    [SerializeField] Button mainButton;
    [SerializeField] RewardType rewardType;

    private void Start()
    {
        eaText.text = $"X {ea}";
        costText.text = cost.ToString("N0");
        switch (costType)
        {
            case CostType.Gold:
                mainButton.onClick.RemoveAllListeners();
                mainButton.onClick.AddListener(GoldBuy);
                break;
            case CostType.Cash:
                mainButton.onClick.RemoveAllListeners();
                mainButton.onClick.AddListener(CashBuy);
                break;
        }
    }
    private async void GoldBuy()
    {
        //팝업켜기 킨후 아래행동
        var userDataManager = UserDataManager.Instance;
        if(userDataManager.CurrentUserData.Gold < cost)
        {
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
                break;
        }
    }
    private async void CashBuy()
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
                break;
        }
    }
}
