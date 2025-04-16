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

    // public bool isUnlock = false;
}
