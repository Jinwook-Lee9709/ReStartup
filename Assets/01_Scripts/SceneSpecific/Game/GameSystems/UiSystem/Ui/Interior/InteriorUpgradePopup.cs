using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InteriorUpgradePopup : MonoBehaviour
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

    private InteriorCard currentCard;
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

    public void SetInfo(InteriorCard card)
    {
        IsPaid = false;
        currentCard = card;
        priceText.text = card.Data.GetSellingCost().ToString();
        var data = currentCard.Data;
        var currentInteriorLevel = UserDataManager.Instance.CurrentUserData.InteriorSaveData[data.InteriorID];
        var beforeSprite = Addressables.LoadAssetAsync<Sprite>(data.IconID + currentInteriorLevel).WaitForCompletion();
        var afterSprite = Addressables.LoadAssetAsync<Sprite>($"{data.IconID}{currentInteriorLevel + 1}").WaitForCompletion();
        beforeIcon.sprite = beforeSprite;
        afterIcon.sprite = afterSprite;
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
