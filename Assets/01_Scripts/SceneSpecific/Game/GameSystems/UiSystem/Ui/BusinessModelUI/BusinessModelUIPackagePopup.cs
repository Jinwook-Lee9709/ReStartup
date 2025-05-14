using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEditor.AddressableAssets.GUI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class BusinessModelUIPackagePopup : MonoBehaviour
{
    [SerializeField] private float backgroundOpacity = 0.8f;
    [SerializeField] private Button background;
    [SerializeField] private Button mainButton;
    [SerializeField] private Image panel;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI moneyValue, goldValue, adTicketValue;
    private BusinessModelUIPackageCard currentCard;

    private bool isPaid;

    private bool IsPaid
    {
        get => isPaid;
        set
        {
            isPaid = value;
        }
    }
    public void Start()
    {
        background.onClick.RemoveAllListeners();
        background.onClick.AddListener(OnClose);
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(OnMainButtonTouched);
    }

    public void SetInfo(BusinessModelUIPackageCard card)
    {
        mainButton.interactable = true;
        currentCard = card;
        moneyValue.text = currentCard.moneyValue.ToString("N0");
        goldValue.text = currentCard.goldValue.ToString("N0");
        adTicketValue.text = currentCard.adTicketValue.ToString("N0");
        costText.text = currentCard.cost.ToString("N0");
    }
    private void OnEnable()
    {
        background.interactable = false;
        if (background != null)
        {
            var backgroundImage = background.GetComponent<Image>();
            backgroundImage.FadeInAnimation();
        }

        if (panel != null)
        {
            panel.transform.PopupAnimation(onComplete: () => background.interactable = true);
        }
    }


    private async void OnMainButtonTouched()
    {
        if (currentCard.times <= 0)
        {
            OnClose();
            return;
        }
        var userDataManager = UserDataManager.Instance;
        await userDataManager.AdjustMoneyWithSave(currentCard.moneyValue);
        await userDataManager.AdjustGoldWithSave(currentCard.goldValue);
        await userDataManager.AdjustAdTicketWithSave(currentCard.adTicketValue);
        currentCard.Buy();
        OnClose();
    }

    private void OnClose()
    {
        background.interactable = false;
        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.FadeOutAnimation();
        panel.transform.PopdownAnimation(onComplete: () => { gameObject.SetActive(false); });
    }
}