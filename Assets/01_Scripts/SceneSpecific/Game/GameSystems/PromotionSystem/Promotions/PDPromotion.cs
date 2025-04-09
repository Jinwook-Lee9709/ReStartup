using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDPromotion : PromotionBase
{
    public PDPromotion(PromotionBase promo) : base(promo)
    {
    }

    public override void Excute(BuffManager buffManager, ConsumerManager consumerManager, bool needAd)
    {
        var pd = DataTableManager.Get<ConsumerDataTable>(DataTableIds.Consumer.ToString()).GetConsumerData(PromotionEffect);
        Buff staffWalk = DataTableManager.Get<BuffDataTable>("Buff").GetBuffForBuffID(pd.BuffID1);
        Buff staffMove = DataTableManager.Get<BuffDataTable>("Buff").GetBuffForBuffID(pd.BuffID2);
        staffWalk.Init();
        staffMove.Init();
        if (consumerManager.CanSpawnConsumer())
        {
            buffManager.StartBuff(staffWalk, () =>
            {
                consumerManager.SpawnConsumer(pd);
                buffManager.StartBuff(staffMove);
                LimitCounting(needAd);
            }, needAd);
        }
    }
}
