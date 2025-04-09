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
    public int PromotionID {  get; set; }
    public PromotionType PromotionType { get; set; }
    public int PromotionEffect { get; set; }
    public CostType CostType { get; set; }
    public int CostQty { get; set; }
    public int LimitBuy { get; set; }
    public int LimitAD { get; set; }

    public int currentLimitBuy;
    public int currentLimitAD;
    public PromotionUI promotionUi;
    public PromotionBase()
    {

    }


    public virtual void Excute(BuffManager buffManager, bool needAd)
    {
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
    }
}
