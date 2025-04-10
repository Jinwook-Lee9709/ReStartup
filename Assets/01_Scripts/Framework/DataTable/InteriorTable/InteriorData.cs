using System;

public class InteriorData
{
    public string Name { get; set; }
    public int InteriorID { get; set; }
    public int RestaurantType { get; set; }
    public ObjectArea CookwareType { get; set; }
    public int SellingCost { get; set; }
    public int Requirements1 { get; set; }
    public int Requirements2 { get; set; }
    public int MaxUpgradeCount { get; set; }
    public int Reward { get; set; }
    public InteriorEffectType EffectType { get; set; }
    public int EffectQuantity { get; set; }
    public int StringID { get; set; }
    public string IconID { get; set; }
    public InteriorCategory Category { get; set; }

    public int GetSellingCost()
    {
        var userData = UserDataManager.Instance.CurrentUserData;
        var upgradeLevel = userData.InteriorSaveData[InteriorID];
        return (int)Math.Round(SellingCost * (1.5f + 0.3f * (upgradeLevel - 1)));
    }

    public bool CheckFirstRequirement()
    {
        return UserDataManager.Instance.CurrentUserData.CurrentRankPoint >= Requirements1;
    }

    public bool CheckSecondRequirement()
    {
        if (Requirements2 == 0)
            return true;
        return UserDataManager.Instance.CurrentUserData.InteriorSaveData[Requirements2] != 0;
    }
}