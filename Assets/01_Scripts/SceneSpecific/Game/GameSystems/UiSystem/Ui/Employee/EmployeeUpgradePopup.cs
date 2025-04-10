using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmployeeUpgradePopup : MonoBehaviour
{
    [SerializeField] private float backgroundOpacity = 0.8f;
    [SerializeField] private Button background;
    [SerializeField] private Button mainButton;
    [SerializeField] private Image panel;
    [SerializeField] private Image beforeIcon;
    [SerializeField] private Image afterIcon;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI mainButtonText;
    [SerializeField] private TextMeshProUGUI priceText;

    private FoodResearchUIItem currentCard;
    private bool isPaid;

    private bool IsPaid
    {
        get => isPaid;
        set
        {
            isPaid = value;
            mainButtonText.text = value ? "확인" : "업그레이드";
        }
    }
    public void Start()
    {
        background.onClick.RemoveAllListeners();
        background.onClick.AddListener(OnClose);
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(OnMainButtonTouched);
    }

    public void SetInfo(FoodResearchUIItem card)
    {
        IsPaid = false;
        currentCard = card;
        priceText.text = card.foodData.BasicCost.ToString();
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
