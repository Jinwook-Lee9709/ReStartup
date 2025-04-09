using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefPromotion : PromotionBase
{
    public ChefPromotion(PromotionBase promo)
    {
        PromotionID = promo.PromotionID;
        PromotionType = promo.PromotionType;
        PromotionEffect = promo.PromotionEffect;
        CostType = promo.CostType;
        CostQty = promo.CostQty;
        LimitBuy = promo.LimitBuy;
        LimitAD = promo.LimitAD;
    }

    public override void Excute(BuffManager buffManager, ConsumerManager consumerManager, bool needAd)
    {
        var chef = DataTableManager.Get<ConsumerDataTable>(DataTableIds.Consumer.ToString()).GetConsumerData(PromotionEffect);
        Buff doubleConsumerBuff = DataTableManager.Get<BuffDataTable>("Buff").GetBuffForBuffID(chef.BuffID1);
        doubleConsumerBuff.Init();
        if (consumerManager.CanSpawnConsumer())
        {
            buffManager.StartBuff(doubleConsumerBuff, () =>
            {
                consumerManager.SpawnConsumer(chef);
                LimitCounting(needAd);
            }, needAd);
        }
    }
}
