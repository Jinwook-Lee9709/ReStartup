using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SupervisorInfoCard : MonoBehaviour
{
    public static readonly string HiredStringID = "Hired";
    
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image moneyImage;
    [SerializeField] private Image supervisorImage;
    [SerializeField] private Button buyButton;
    private SupervisorInfo supervisorInfo;

    public void Init(SupervisorInfo info, Action<int> onBuy)
    {
        supervisorInfo = info;
        InitHireButton(onBuy);
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        nameText.text = supervisorInfo.name;
        supervisorImage.sprite = supervisorInfo.icon;
        SetCostText(supervisorInfo.isHired);
    }

    private void InitHireButton(Action<int> onBuy)
    {
        SetHireButton();
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(
            () =>
            {
                onBuy?.Invoke(supervisorInfo.number);
                OnBuy();
            });

    }

    private void SetHireButton()
    {
        buyButton.gameObject.SetActive(!supervisorInfo.isHired && supervisorInfo.isHireable);
        buyButton.interactable = supervisorInfo.cost < UserDataManager.Instance.CurrentUserData.Money;
    }

    public void ChangeInfo(SupervisorInfo info)
    {
        supervisorInfo = info;
        
        SetHireButton();
        UpdateDisplay();
    }

    public void OnMoneyChanged(int money)
    {
        SetCostText(supervisorInfo.isHired);
        if (supervisorInfo.isHired || !supervisorInfo.isHireable)
            return;
        
        buyButton.interactable = supervisorInfo.cost < money;
    }
    
    public void ChangeToHireable()
    {
        supervisorInfo.isHireable = true;
        buyButton.gameObject.SetActive(true);
        OnMoneyChanged((int)UserDataManager.Instance.CurrentUserData.Money);
    }

    public void OnBuy()
    {
        SetCostText(true);
        UserDataManager.Instance.AdjustMoneyWithSave(-supervisorInfo.cost).Forget();
        buyButton.gameObject.SetActive(false);
    }

    public void SetCostText(bool isHired)
    {
        if (isHired)
        {
            var hiredString = LZString.GetUIString(HiredStringID);
            costText.text = hiredString;
            costText.color = Color.black;
        }
        else
        {
            costText.text = supervisorInfo.cost.ToString();
            costText.color = UserDataManager.Instance.CurrentUserData.Money >= supervisorInfo.cost ? Color.black : Color.red;
        }
    
    }
}
