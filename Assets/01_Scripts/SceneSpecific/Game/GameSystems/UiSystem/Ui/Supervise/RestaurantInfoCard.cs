using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RestaurantInfoCard : MonoBehaviour
{
    public enum ShopType
    {
        Previous,
        Current,
        Next,
        Post
    }
    
    [SerializeField] private TextMeshProUGUI shopNameText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private GameObject alertPanel;
    [SerializeField] private Image shopImage;
    [SerializeField] private Button buyButton;
    [SerializeField] private Transform starParent;
    [SerializeField] private List<Image> starImages;
    private ShopType shopType = ShopType.Post;
    private int cost;
    
    public void SetInfo(string shopName, int cost, Sprite shopImage, ShopType type)
    {
        shopNameText.text = shopName;
        costText.text = cost.ToString();
        this.shopImage.sprite = shopImage;
        this.cost = cost;
        SetLayout(type);
        UpdateInteractable();
    }

    public void SetLayout(ShopType type)
    {
        shopType = type;
        starParent.gameObject.SetActive(false);
        buyPanel.SetActive(false);
        alertPanel.SetActive(false);
        switch (type)
        {
            case ShopType.Previous:
                break;
            case ShopType.Current:
                starParent.gameObject.SetActive(true);
                break;
            case ShopType.Next:
                buyPanel.gameObject.SetActive(true);
                break;
            case ShopType.Post:
                alertPanel.SetActive(true);
                break;
        }
    }
    
    public void RegisterAction(UnityAction action)
    {
        buyButton.gameObject.SetActive(true);
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(action);
    }

    public void UpdateInteractable()
    {
        buyButton.interactable = CheckCondition();
    }

    public bool CheckCondition()
    {
        if (shopType != ShopType.Current)
            return false;
        var currentTheme = ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;
        bool isMoneyEnough = cost < UserDataManager.Instance.CurrentUserData.Money;
        bool isManagerHired = UserDataManager.Instance.CurrentUserData.ThemeStatus[currentTheme].managerCount != 0;
        // buyButton.interactable = isMoneyEnough && isManagerHired;
        return true;
    }
}
