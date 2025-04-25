using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static SoonsoonData;

public class FoodUpgradePopup : MonoBehaviour
{
    [SerializeField] private float backgroundOpacity = 0.8f;
    [SerializeField] private Button background;
    [SerializeField] private Button mainButton;
    [SerializeField] private Image panel;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI mainButtonText;
    [SerializeField] private TextMeshProUGUI priceText;

    private FoodUpgradeUIItem currentCard;
    private bool isPaid;

    private bool IsPaid
    {
        get => isPaid;
        set
        {
            isPaid = value;
            mainButtonText.text = value ? LZString.GetUIString(Strings.Check) : LZString.GetUIString(Strings.Upgrade);
        }
    }
    public void Start()
    {
        background.onClick.RemoveAllListeners();
        background.onClick.AddListener(OnClose);
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(OnMainButtonTouched);
    }

    public void SetInfo(FoodUpgradeUIItem card)
    {
        IsPaid = false;
        currentCard = card;
        icon.sprite = card.image.sprite;
        infoText.text = LZString.GetUIString(string.Format(Strings.foodNameKeyFormat,card.foodData.StringID));
        priceText.text = (card.foodData.BasicCost * (card.foodData.upgradeCount + 1)).ToString();
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
            panel.transform.PopupAnimation(onComplete:()=> background.interactable = true);
        }
    }


    private void OnMainButtonTouched()
    {
        if (currentCard.foodData.upgradeCount < 5)
        {
            currentCard.OnBuy();
            //업그레이드 완료 텍스트 띄워주기
        }
        else
        {
            OnClose();
        }
    }

    private void OnClose()
    {
        background.interactable = false;
        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.FadeOutAnimation();
        panel.transform.PopdownAnimation(onComplete: () => { gameObject.SetActive(false); });
    }
}
