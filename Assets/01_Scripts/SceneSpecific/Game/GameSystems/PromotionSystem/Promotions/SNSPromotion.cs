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
        if(UserDataManager.Instance.CurrentUserData.Money < CostQty)
        {
            GameObject.Instantiate(notEnoughCost, parentCanvas.transform);
            return;
        }
        base.Excute(buffManager, needAd);
        Buff footTrafficBuff = DataTableManager.Get<BuffDataTable>("Buff").GetBuffForBuffID(PromotionEffect);
        footTrafficBuff.Init();
        buffManager.StartBuff(footTrafficBuff, () =>
        {
            LimitCounting(needAd);
            OnPayment(needAd);

        }, needAd);
    }
}
