using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SupervisorInfoCard : MonoBehaviour
{
    public static readonly string HiredStringID = "Hired";
    
    private TextMeshProUGUI nameText;
    private TextMeshProUGUI costText;
    private Image moneyImage;
    private Button buyButton;

    public void Init(string supervisorName, int cost, bool isHired, Action onBuy)
    {
        nameText.text = supervisorName;
        
        if (isHired)
        {
            SetHiredText();
        }
        SetHireButton(isHired, onBuy);
    }

    private void SetHireButton(bool isHired, Action onBuy)
    {
        buyButton.gameObject.SetActive(!isHired);
        if (!isHired)
        {
            buyButton.onClick.AddListener(
                () =>
                {
                    onBuy?.Invoke();
                    OnBuy();
                });
        }
    }


    public void UpdateInteractable(bool isInteractable)
    {
        buyButton.interactable = isInteractable;
    }

    public void OnBuy()
    {
        SetHiredText();
    }

    public void SetHiredText()
    {
        var hiredString = LZString.GetUIString(HiredStringID);
        costText.text = hiredString;
    }
    
}
