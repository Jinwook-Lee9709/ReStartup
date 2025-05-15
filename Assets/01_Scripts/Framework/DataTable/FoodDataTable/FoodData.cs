public class FoodData
{
    public int FoodID { get; set; }
    public int Type { get; set; }
    public CookwareType CookwareType { get; set; }
    public int CookwareNB { get; set; }
    public int SellingCost { get; set; }
    public int Requirements { get; set; }
    public int BasicCost { get; set; }
    public int GetRankPoints { get; set; }
    public int StringID { get; set; }
    public string IconID { get; set; }

    public int upgradeCount;
    private int basicSellingCost;
    private float upgradeSellingCostValue;
    public void BasicSellingCostSet()
    {
        basicSellingCost = SellingCost;
    }
    public void SetSellingCost()
    {
        switch (upgradeCount)
        {
            case 1:
                upgradeSellingCostValue = 1f;
                break;
            case 2:
                upgradeSellingCostValue = 1.2f;
                break;
            case 3:
                upgradeSellingCostValue = 1.5f;
                break;
            case 4:
                upgradeSellingCostValue = 1.7f;
                break;
            case 5:
                upgradeSellingCostValue = 2f;
                break;
        }
        SellingCost = (int)((float)basicSellingCost * upgradeSellingCostValue);
    }

    // public bool isUnlock = false;
}
