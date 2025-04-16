using Cysharp.Threading.Tasks;
using System;

public enum CostType
{
    Free,
    Money,
    Gold,
}
public interface IPromotion
{
    public abstract void Init();
    public abstract void Excute(BuffManager buffManager, bool needAd);
    public abstract void Excute(BuffManager buffManager, ConsumerManager consumerManager, bool needAd);
    public abstract void LimitCounting(bool needAd);
}


public class PromotionBase : IPromotion
{
    public int PromotionID { get; set; }
    public PromotionType PromotionType { get; set; }
    public int PromotionEffect { get; set; }
    public CostType CostType { get; set; }
    public int CostQty { get; set; }
    public int LimitBuy { get; set; }
    public int LimitAD { get; set; }
    public string PromotionIcon { get; set; }

    public int currentLimitBuy;
    public int currentLimitAD;
    public PromotionUI promotionUi;
    public PromotionBase()
    {

    }
    public PromotionBase(PromotionBase promo)
    {
        PromotionID = promo.PromotionID;
        PromotionType = promo.PromotionType;
        PromotionEffect = promo.PromotionEffect;
        CostType = promo.CostType;
        CostQty = promo.CostQty;
        LimitBuy = promo.LimitBuy;
        LimitAD = promo.LimitAD;
        PromotionIcon = promo.PromotionIcon;
    }

    public virtual void Excute(BuffManager buffManager, bool needAd)
    {
        UserDataManager.Instance.OnRankPointUp(10);
    }

    public virtual void Init()
    {
        currentLimitBuy = LimitBuy;
        currentLimitAD = LimitAD;
    }

    public virtual void LimitCounting(bool needAd)
    {
        if (needAd)
        {
            currentLimitAD--;
        }
        else
        {
            currentLimitBuy--;
        }
        promotionUi.UpdateUI();
    }

    public virtual void Excute(BuffManager buffManager, ConsumerManager consumerManager, bool needAd)
    {
        UserDataManager.Instance.OnRankPointUp(10);
    }

    public void OnPayment(bool needAd)
    {
        if(!needAd)
        {
            switch (CostType)
            {
                case CostType.Money:
                    UserDataManager.Instance.AdjustMoneyWithSave(CostQty).Forget();
                    break;
                case CostType.Gold:
                    break;
            }
        }
    }
}
