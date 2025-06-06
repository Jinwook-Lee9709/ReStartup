using System;
using Excellcube.EasyTutorial.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PromotionManager : MonoBehaviour
{
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private GameObject notEnoughCostPopup;
    [SerializeField] private BuffManager buffManager;
    [SerializeField] private ConsumerManager consumerManager;
    [SerializeField] private GameObject promotionContent;
    [SerializeField] private GameObject promotionPrefab;
    [SerializeField] private TextMeshProUGUI haveMeney;
    [SerializeField] private TextMeshProUGUI haveGold;
    private List<PromotionBase> promotions = new();
    private List<PromotionUI> promotionsUIs = new();

    private void Start()
    {
        foreach (var promotion in DataTableManager.Get<PromotionDataTable>(DataTableIds.Promoiton.ToString()).GetData())
        {
            PromotionBase promotionBase = null;
            switch (promotion.Value.PromotionType)
            {
                case PromotionType.SNS:
                    promotionBase = new SNSPromotion(promotion.Value);
                    break;
                case PromotionType.Ituber:
                    promotionBase = new ItuberPromotion(promotion.Value);
                    break;
                case PromotionType.PD:
                    promotionBase = new PDPromotion(promotion.Value);
                    break;
                case PromotionType.Chef:
                    promotionBase = new ChefPromotion(promotion.Value);
                    break;
            }

            if (promotionBase != null)
            {
                promotionBase.Init();
                if (UserDataManager.Instance.CurrentUserData.PromotionSaveData.ContainsKey(promotionBase.PromotionID))
                {
                    promotionBase.currentLimitAD -= UserDataManager.Instance.CurrentUserData.PromotionSaveData[promotionBase.PromotionID].adUseCount;
                    promotionBase.currentLimitBuy -= UserDataManager.Instance.CurrentUserData.PromotionSaveData[promotionBase.PromotionID].buyUseCount;
                }
               
                promotions.Add(promotionBase);
                promotionBase.notEnoughCost = notEnoughCostPopup;
                promotionBase.parentCanvas = parentCanvas;
            }
            
        }

        var currentThemePromotionConsumerList = DataTableManager.Get<ConsumerDataTable>(DataTableIds.Consumer.ToString()).GetPromotionConsumerDataForCurrentTheme(ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme);
        foreach (var promotion in promotions)
        {
            if (promotion.PromotionType == PromotionType.SNS || currentThemePromotionConsumerList.Contains(DataTableManager.Get<ConsumerDataTable>(DataTableIds.Consumer.ToString()).GetConsumerData(promotion.PromotionEffect)))
            {
                var promotionUi = Instantiate(promotionPrefab).GetComponent<PromotionUI>();
                promotionUi.Init(promotion);
                
                if (promotionUi.promotionData.PromotionType == PromotionType.SNS)
                    promotionUi.payButton.gameObject.AddComponent<TutorialSelectionTarget>().Key = "PromoitonTutorial";

                promotionUi.transform.SetParent(promotionContent.transform);
                promotionUi.transform.localScale = Vector3.one;
                promotionUi.buffManager = buffManager;
                promotionUi.consumerManager = consumerManager;
                promotion.promotionUi = promotionUi;
                promotionsUIs.Add(promotionUi);
                promotionUi.UpdateUI();
            }
        }
        haveMeney.text = UserDataManager.Instance.CurrentUserData.Money.ToString();
        haveGold.text = UserDataManager.Instance.CurrentUserData.Gold.ToString();
        UserDataManager.Instance.ChangeMoneyAction += ChangeGoods;
    }
    public void ChangeGoods(int? a)
    {
        haveMeney.text = a.ToString();
        haveGold.text = UserDataManager.Instance.CurrentUserData.Gold.ToString();
    }

    private void OnDestroy()
    {
        if( UserDataManager.Instance != null)
            UserDataManager.Instance.ChangeMoneyAction -= ChangeGoods;
    }
}
