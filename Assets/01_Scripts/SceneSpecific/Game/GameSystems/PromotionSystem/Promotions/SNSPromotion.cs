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
        if(UserDataManager.Instance.CurrentUserData.Money < CostQty && !needAd)
        {
            GameObject.Instantiate(notEnoughCost, parentCanvas.transform);
            return;
        }
        Buff footTrafficBuff = DataTableManager.Get<BuffDataTable>("Buff").GetBuffForBuffID(PromotionEffect);
        footTrafficBuff.Init();
        buffManager.StartBuff(footTrafficBuff, async () =>
        {
            base.Excute(buffManager, needAd);
            LimitCounting(needAd);
            OnPayment(needAd);
        }, needAd);
    }
}
