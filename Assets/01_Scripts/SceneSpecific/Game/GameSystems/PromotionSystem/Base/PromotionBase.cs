using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public enum CostType
{
    Free,
    Money,
    Gold,
    Cash,
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
    public string PromotionEffectText { get; set; }

    public GameObject notEnoughCost;
    public Canvas parentCanvas;

    public int currentLimitBuy;
    public int currentLimitAD;
    public PromotionUI promotionUi;

    public string promotionName;
    public string promotionDescription;

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
        PromotionEffectText = promo.PromotionEffectText;
    }

    public virtual void Excute(BuffManager buffManager, bool needAd)
    {
        UserDataManager.Instance.AddRankPointWithSave(10).Forget();
        ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.Promotion, 1, PromotionID);
        ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager.OnEventInvoked(MissionMainCategory.Promotion, 1);
    }

    public virtual void Init()
    {
        currentLimitBuy = LimitBuy;
        currentLimitAD = LimitAD;
        promotionName = LZString.GetUIString(string.Format(Strings.promotionNameFormat, PromotionID));
        promotionDescription = LZString.GetUIString(string.Format(Strings.promotionDescriptionFormat, PromotionID));
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
        UserDataManager.Instance.OnUsePromotion(this, needAd).Forget();
    }

    public virtual void Excute(BuffManager buffManager, ConsumerManager consumerManager, bool needAd)
    {
        UserDataManager.Instance.AddRankPointWithSave(10).Forget();
    }

    public void OnPayment(bool needAd)
    {
        if(!needAd)
        {
            switch (CostType)
            {
                case CostType.Money:
                    UserDataManager.Instance.AdjustMoneyWithSave(-CostQty).Forget();
                    break;
                case CostType.Gold:
                    UserDataManager.Instance.AdjustGoldWithSave(-CostQty).Forget();
                    break;
            }
        }
    }
}
