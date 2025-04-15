using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItuberPromotion : PromotionBase
{
    public ItuberPromotion(PromotionBase promo) : base(promo)
    {
    }

    public override void Excute(BuffManager buffManager, ConsumerManager consumerManager, bool needAd)
    {
        base.Excute(buffManager, consumerManager, needAd);
        var ituber = DataTableManager.Get<ConsumerDataTable>(DataTableIds.Consumer.ToString()).GetConsumerData(PromotionEffect);
        Buff footTrafficBuff = DataTableManager.Get<BuffDataTable>("Buff").GetBuffForBuffID(ituber.BuffID1);
        footTrafficBuff.Init();
        if (consumerManager.CanSpawnConsumer())
        {
            buffManager.StartBuff(footTrafficBuff, () =>
            {
                consumerManager.SpawnConsumer(ituber);
                LimitCounting(needAd);
            }, needAd);
        }
        else
        {
            buffManager.StartBuff(footTrafficBuff, () =>
            {
                consumerManager.AddPromotionConsumerWaitingLine(ituber);
                LimitCounting(needAd);
            }, needAd);
        }
    }
}
