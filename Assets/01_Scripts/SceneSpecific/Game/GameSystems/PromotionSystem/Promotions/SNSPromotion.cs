using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SNSPromotion : PromotionBase
{
    public SNSPromotion(PromotionBase promo) : base(promo)
    {
    }

    public override void Excute(BuffManager buffManager, bool needAd)
    {
        base.Excute(buffManager, needAd);
        Buff footTrafficBuff = DataTableManager.Get<BuffDataTable>("Buff").GetBuffForBuffID(PromotionEffect);
        footTrafficBuff.Init();
        buffManager.StartBuff(footTrafficBuff, () => LimitCounting(needAd), needAd);
    }
}
