using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefPromotion : PromotionBase
{
    public ChefPromotion(PromotionBase promo) : base(promo)
    {
    }

    public override void Excute(BuffManager buffManager, ConsumerManager consumerManager, bool needAd)
    {
        if (UserDataManager.Instance.CurrentUserData.Gold < CostQty)
        {
            GameObject.Instantiate(notEnoughCost, parentCanvas.transform);
            return;
        }
        base.Excute(buffManager, consumerManager, needAd);
        var chef = DataTableManager.Get<ConsumerDataTable>(DataTableIds.Consumer.ToString()).GetConsumerData(PromotionEffect);
        Buff doubleConsumerBuff = DataTableManager.Get<BuffDataTable>("Buff").GetBuffForBuffID(chef.BuffId1);
        doubleConsumerBuff.Init();
        if (consumerManager.CanSpawnConsumer())
        {
            buffManager.StartBuff(doubleConsumerBuff, () =>
            {
                consumerManager.SpawnConsumer(chef);
                LimitCounting(needAd);
                OnPayment(needAd);
            }, needAd);
        }
        else
        {
            buffManager.StartBuff(doubleConsumerBuff, () =>
            {
                consumerManager.AddPromotionConsumerWaitingLine(chef);
                LimitCounting(needAd);
                OnPayment(needAd);
            }, needAd);
        }
    }
}
