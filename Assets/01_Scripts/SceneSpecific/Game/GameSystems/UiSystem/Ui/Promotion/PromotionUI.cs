using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromotionUI : MonoBehaviour
{
    private string cntFormat = "{0} / {1}";
    public BuffManager buffManager;
    public ConsumerManager consumerManager;
    public Button payButton;
    public Button adButton;
    public PromotionBase promotionData;
    public TextMeshProUGUI buyCntText, adCntText, promotionNameText, promotionEffectText;
    private void Start()
    {
        payButton.onClick.AddListener(OnPayButtonClick);
        adButton.onClick.AddListener(OnAdButtonClick);
    }
    public void Init(PromotionBase promotion)
    {
        promotionData = promotion;
        promotionNameText.text = $"{promotionData.PromotionType.ToString()}";
        UpdateUI();
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
        buyCntText.text = string.Format(cntFormat, promotionData.currentLimitBuy, promotionData.LimitBuy);
        adCntText.text = string.Format(cntFormat, promotionData.currentLimitAD, promotionData.LimitAD);
    }
}
