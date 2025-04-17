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
        if (UserDataManager.Instance.CurrentUserData.Money < CostQty && !needAd)
        {
            GameObject.Instantiate(notEnoughCost, parentCanvas.transform);
            return;
        }
        var ituber = DataTableManager.Get<ConsumerDataTable>(DataTableIds.Consumer.ToString()).GetConsumerData(PromotionEffect);
        Buff footTrafficBuff = DataTableManager.Get<BuffDataTable>("Buff").GetBuffForBuffID(ituber.BuffId1);
        footTrafficBuff.Init();
        if (consumerManager.CanSpawnConsumer())
        {
            buffManager.StartBuff(footTrafficBuff, () =>
            {
                base.Excute(buffManager, consumerManager, needAd);
                consumerManager.SpawnConsumer(ituber);
                LimitCounting(needAd);
                OnPayment(needAd);
            }, needAd);
        }
        else
        {
            buffManager.StartBuff(footTrafficBuff, () =>
            {
                base.Excute(buffManager, consumerManager, needAd);
                consumerManager.AddPromotionConsumerWaitingLine(ituber);
                LimitCounting(needAd);
                OnPayment(needAd);
            }, needAd);
        }
    }
}
