using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class PromotionUI : MonoBehaviour
{
    [SerializeField] private Image promotionImage;
    [SerializeField] private Image costImage;
    public BuffManager buffManager;
    public ConsumerManager consumerManager;
    public Button payButton;
    public Button adButton;
    public PromotionBase promotionData;
    public TextMeshProUGUI buyCntText, adCntText, promotionNameText, promotionEffectText, costText;
    private void Start()
    {
        payButton.onClick.AddListener(OnPayButtonClick);
        adButton.onClick.AddListener(OnAdButtonClick);
    }
    public void Init(PromotionBase promotion)
    {
        promotionData = promotion;
        promotionNameText.text = $"{promotionData.promotionName}";
        promotionEffectText.text = $"{promotionData.promotionDescription}";
        
        var sprite = Addressables.LoadAssetAsync<Sprite>(String.Format(Strings.promotionIconFormat,promotionData.PromotionType)).WaitForCompletion();
        promotionImage.sprite = sprite;
        
        switch (promotionData.CostType)
        {
            case CostType.Free:
                costImage.sprite = LoadCostImage(Strings.Free);
                costText.gameObject.SetActive(false);
                break;
            case CostType.Money:
                costImage.sprite = LoadCostImage(Strings.Cash);
                costText.text = promotionData.CostQty.ToString(Strings.costFormat);
                break;
            case CostType.Gold:
                costImage.sprite = LoadCostImage(Strings.Gold);
                costText.text = promotionData.CostQty.ToString(Strings.costFormat);
                break;
        }
        
        
        UpdateUI();
    }

    private Sprite LoadCostImage(string key)
    {
        var handle = Addressables.LoadAssetAsync<Sprite>(key);
        handle.WaitForCompletion();
        return handle.Result;
    }

    public void OnPayButtonClick()
    {
        if (promotionData.PromotionType == PromotionType.SNS)
        {
            promotionData.Excute(buffManager, false);
        }
        else
        {
            promotionData.Excute(buffManager, consumerManager, false);
        }
    }
    public void OnAdButtonClick()
    {
        if (promotionData.PromotionType == PromotionType.SNS)
        {
            promotionData.Excute(buffManager, true);
        }
        else
        {
            promotionData.Excute(buffManager, consumerManager, true);
        }
    }

    public void UpdateUI()
    {
        payButton.interactable = true;
        adButton.interactable = true;
        if (promotionData.currentLimitAD <= 0)
        {
            promotionData.currentLimitAD = 0;
            adButton.interactable = false;
        }
        if (promotionData.currentLimitBuy <= 0)
        {
            promotionData.currentLimitBuy = 0;
            payButton.interactable = false;
        }
        buyCntText.text = string.Format(Strings.cntFormat, promotionData.currentLimitBuy, promotionData.LimitBuy);
        adCntText.text = string.Format(Strings.cntFormat, promotionData.currentLimitAD, promotionData.LimitAD);
    }
}
