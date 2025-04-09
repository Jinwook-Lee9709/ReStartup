using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SNSPromotion : PromotionBase
{
    public SNSPromotion(PromotionBase promo)
    {
        PromotionID = promo.PromotionID;
        PromotionType = promo.PromotionType;
        PromotionEffect = promo.PromotionEffect;
        CostType = promo.CostType;
        CostQty = promo.CostQty;
        LimitBuy = promo.LimitBuy;
        LimitAD = promo.LimitAD;
    }

    public override void Excute(BuffManager buffManager, bool needAd)
    {
        Buff footTrafficBuff = DataTableManager.Get<BuffDataTable>("Buff").GetBuffForBuffID(PromotionEffect);
        footTrafficBuff.Init();
        buffManager.StartBuff(footTrafficBuff, () => LimitCounting(needAd), needAd);
    }
}
