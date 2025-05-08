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
    
    [SerializeField] private float backgroundOpacity = 0.8f;
    [SerializeField] private Button background;
    [SerializeField] private Button mainButton;
    [SerializeField] private Image panel;
    [SerializeField] private Image beforeIcon;
    [SerializeField] private Image afterIcon;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI priceText;

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
        
        infoText.text = string.Format(UpgradeInfoFormat, beforeString, afterString);
        
        var beforeSprite = Addressables.LoadAssetAsync<Sprite>(data.IconID + currentInteriorLevel).WaitForCompletion();
        var afterSprite = Addressables.LoadAssetAsync<Sprite>($"{data.IconID}{currentInteriorLevel + 1}").WaitForCompletion();
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
            panel.transform.PopupAnimation(onComplete:()=> background.interactable = true);
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
