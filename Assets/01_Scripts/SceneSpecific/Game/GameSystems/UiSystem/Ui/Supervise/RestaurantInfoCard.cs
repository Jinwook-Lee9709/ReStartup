using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RestaurantInfoCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI shopNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Image shopImage;
    [SerializeField] private Button buyButton;

    public void SetInfo(string shopName, int cost, Sprite shopImage)
    {
        shopNameText.text = shopName;
        costText.text = cost.ToString();
        this.shopImage.sprite = shopImage;
    }

    public void RegisterAction(UnityAction action)
    {
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(action);
    }

    public void UpdateInteractable()
    {
        
    }
}
