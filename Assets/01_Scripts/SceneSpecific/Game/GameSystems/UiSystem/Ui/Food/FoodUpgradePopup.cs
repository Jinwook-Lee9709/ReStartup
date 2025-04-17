using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        priceText.text = (card.foodData.BasicCost * card.foodData.upgradeCount).ToString();
    }
    private void OnEnable()
    {
        if (background != null)
        {
            var backgroundImage = background.GetComponent<Image>();
            backgroundImage.FadeInAnimation();
        }

        if (panel != null)
        {
            panel.transform.PopupAnimation();
        }
    }


    private void OnMainButtonTouched()
    {
        if (!IsPaid)
        {
            IsPaid = true;
            currentCard.OnBuy();
        }
        else
        {
            OnClose();
        }
    }

    private void OnClose()
    {
        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.FadeOutAnimation();
        panel.transform.PopdownAnimation(onComplete: () => { gameObject.SetActive(false); });
    }
}
