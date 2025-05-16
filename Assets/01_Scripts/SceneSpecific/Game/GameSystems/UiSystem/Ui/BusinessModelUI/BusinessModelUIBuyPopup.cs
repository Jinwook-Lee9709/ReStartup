using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class BusinessModelUIBuyPopup : MonoBehaviour
{
    private readonly string goldExplanation = "GoldExplanationBusinessPopup{0}";
    private readonly string moneyExplanation = "MoneyExplanationBusinessPopup{0}";
    private readonly string adTicketExplanation = "AdTicketExplanationBusinessPopup{0}";
    [SerializeField] private float backgroundOpacity = 0.8f;
    [SerializeField] private Button background;
    [SerializeField] private Button mainButton;
    [SerializeField] private Image panel;
    [SerializeField] private Image Icon;
    [SerializeField] private Image costImage;
    [SerializeField] private TextMeshProUGUI explanationText;
    [SerializeField] private TextMeshProUGUI costText;
    private BusinessModelUICard currentCard;

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

    public void SetInfo(BusinessModelUICard card, Sprite image, Sprite costimage)
    {
        mainButton.interactable = true;
        currentCard = card;
        costText.text = currentCard.cost.ToString("N0");
        Icon.sprite = image;
        costImage.sprite = costimage;
        switch (currentCard.rewardType)
        {
            case RewardType.Money:
                explanationText.text = LZString.GetUIString(string.Format(moneyExplanation, currentCard.ea));
                break;
            case RewardType.Gold:
                explanationText.text = LZString.GetUIString(string.Format(goldExplanation, currentCard.ea));
                break;
            case RewardType.AdBlockTicket:
                explanationText.text = LZString.GetUIString(string.Format(adTicketExplanation, currentCard.ea));
                break;
        }
    }
    private void OnEnable()
    {
        background.interactable = false;
        mainButton.interactable = true;
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


    private void OnMainButtonTouched()
    {
        mainButton.interactable = false;
        switch (currentCard.costType)
        {
            case CostType.Free:
                break;
            case CostType.Money:
                break;
            case CostType.Gold:
                mainButton.interactable = false;
                currentCard.GoldBuy();
                break;
            case CostType.Cash:
                mainButton.interactable = false;
                currentCard.CashBuy();
                break;
        }
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