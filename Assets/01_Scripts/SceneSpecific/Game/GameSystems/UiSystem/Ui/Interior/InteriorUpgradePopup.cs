using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InteriorUpgradePopup : MonoBehaviour
{
    private static readonly string UpgradeInfoFormat = "{0} -> {1}";
    private static readonly string UpgradeEffectWithPercentFormat = "{0}% -> {1}%";

    [SerializeField] private float backgroundOpacity = 0.8f;
    [SerializeField] private Button background;
    [SerializeField] private Button mainButton;
    [SerializeField] private Image panel;
    [SerializeField] private Image beforeIcon;
    [SerializeField] private Image afterIcon;
    [SerializeField] private TextMeshProUGUI prevProductName;
    [SerializeField] private TextMeshProUGUI nextProductName;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI effectNameText;
    [SerializeField] private TextMeshProUGUI effectAmountText;

    private InteriorCard currentCard;

    public void Start()
    {
        background.onClick.RemoveAllListeners();
        background.onClick.AddListener(OnClose);
        mainButton.onClick.RemoveAllListeners();
        mainButton.onClick.AddListener(OnMainButtonTouched);
    }

    public void SetInfo(InteriorCard card)
    {
        currentCard = card;
        var data = currentCard.Data;
        priceText.text = data.GetSellingCost().ToString();
        var currentInteriorLevel = UserDataManager.Instance.CurrentUserData.InteriorSaveData[data.InteriorID];

        String beforeString = LZString.GetUIString(data.StringID + currentInteriorLevel);
        String afterString = LZString.GetUIString($"{data.StringID}{currentInteriorLevel + 1}");
        String effectString = LZString.GetUIString(data.EffectType.ToString());
        String effectAmountString = string.Empty;
        bool isRank = data.EffectType == InteriorEffectType.RankPoints; 
            effectAmountString = string.Format(isRank? UpgradeInfoFormat : UpgradeEffectWithPercentFormat, data.EffectQuantity * currentInteriorLevel,
            data.EffectQuantity * (currentInteriorLevel + 1));
        effectNameText.text = effectString;
        effectAmountText.text = effectAmountString;
        prevProductName.text = beforeString;
        nextProductName.text = afterString;

        var beforeSprite = Addressables.LoadAssetAsync<Sprite>(data.IconID + currentInteriorLevel).WaitForCompletion();
        var afterSprite = Addressables.LoadAssetAsync<Sprite>($"{data.IconID}{currentInteriorLevel + 1}")
            .WaitForCompletion();
        beforeIcon.sprite = beforeSprite;
        afterIcon.sprite = afterSprite;
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


    private void OnMainButtonTouched()
    {
        MainButtonInputHandleTask().Forget();
    }

    private async UniTask MainButtonInputHandleTask()
    {
        OnClose();
        await currentCard.OnBuy();
    }

    private void OnClose()
    {
        background.interactable = false;
        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.FadeOutAnimation();
        panel.transform.PopdownAnimation(onComplete: () => { gameObject.SetActive(false); });
    }
}