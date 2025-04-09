using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionManager : MonoBehaviour
{
    [SerializeField] private BuffManager buffManager;
    [SerializeField] private ConsumerManager consumerManager;
    [SerializeField] private GameObject promotionContent;
    [SerializeField] private GameObject promotionPrefab;
    private List<PromotionBase> promotions = new();
    private List<PromotionUI> promotionsUIs = new();

    private void Start()
    {
        foreach (var promotion in DataTableManager.Get<PromotionDataTable>(DataTableIds.Promoiton.ToString()).GetData())
        {
            switch (promotion.Value.PromotionType)
            {
                case PromotionType.SNS:
                    var snsPromo = new SNSPromotion(promotion.Value);
                    promotions.Add(snsPromo);
                    break;
                case PromotionType.Ituber:
                    var ituberPromo = new ItuberPromotion(promotion.Value);
                    promotions.Add(ituberPromo);
                    break;
                case PromotionType.PD:
                    var pdPromo = new PDPromotion(promotion.Value);
                    promotions.Add(pdPromo);
                    break;
                case PromotionType.Chef:
                    var chefPromo = new ChefPromotion(promotion.Value);
                    promotions.Add(chefPromo);
                    break;
            }
        }

        var currentThemePromotionConsumerList = DataTableManager.Get<ConsumerDataTable>(DataTableIds.Consumer.ToString()).GetPromotionConsumerDataForCurrentTheme(ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme);
        foreach (var promotion in promotions)
        {
            if (promotion.PromotionType == PromotionType.SNS || currentThemePromotionConsumerList.Contains(DataTableManager.Get<ConsumerDataTable>(DataTableIds.Consumer.ToString()).GetConsumerData(promotion.PromotionEffect)))
            {
                promotion.Init();
                var promotionUi = Instantiate(promotionPrefab).GetComponent<PromotionUI>();
                promotionUi.Init(promotion);
                promotionUi.transform.SetParent(promotionContent.transform);
                promotionUi.transform.localScale = Vector3.one;
                promotionUi.buffManager = buffManager;
                promotionUi.consumerManager = consumerManager;
                promotion.promotionUi = promotionUi;
                promotionsUIs.Add(promotionUi);
            }
        }
    }



}
