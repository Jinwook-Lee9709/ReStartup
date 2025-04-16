using System;

[Flags]
public enum DebugFlags
{
    None = 0,
    FrameRate = 1 << 0,
    WorkSystem = 1 << 1
}

public enum CurrencyType
{
    Money,
    Gold
}

public enum DataTableIds
{
    Employee,
    Food,
    Consumer,
    Ranking,
    Interior,
    Cookware,
    Buff,
    Promoiton,
    rankCondition,
    periodQuest,
}

public enum IconPivots
{
    Default,
    Consumer
}

public enum ThemeIds
{
    Theme1 = 1,
    Theme2,
    Theme3
}

public enum WorkType
{
    All,
    Payment = 1,
    Hall,
    Kitchen
}

public enum WorkStatus
{
    Waiting,
    Assigned
}

public enum InteractStatus
{
    Pending,
    Progressing,
    Success
}

[Flags]
public enum InteractPermission
{
    None = 0,
    Consumer = 1 << 0,
    PaymentEmployee = 1 << 1,
    KitchenEmployee = 1 << 2,
    HallEmployee = 1 << 3,
    Player = 1 << 4
}

public enum LanguageType
{
    Korean
}

public enum SceneIds
{
    Dev0 = 1,
    Dev1,
    Dev2,
    Dev3,
    Theme1,
    Theme2,
    Theme3,
    Title,
    Lobby,
    Loading,


}

public enum BuffType
{
    FootTraffic,
    StaffWork,
    StaffMove,
    PairSpawn,
    TimerSpeed
}

public enum PromotionType
{
    SNS = -1,
    Ituber,
    PD,
    Chef
}

public enum QuestType
{
    Daily,
    Weekly
}

public enum RequirementsType
{
    SoldFood,
    MakingFood
}

public static class Endpoints
{
    // private static readonly string BaseUrl = "https://localhost:443/api";
    private static readonly string BaseUrl = "https://ec2-3-39-166-105.ap-northeast-2.compute.amazonaws.com:3000/api";

    public static readonly string DeleteUserUrl = BaseUrl + "/auth/delete";

    public static readonly string GuestLoginUrl = BaseUrl + "/auth/guestLogin";
    public static readonly string GuestRegisterUrl = BaseUrl + "/auth/guestRegister";
    public static readonly string VerifyTokenUrl = BaseUrl + "/auth/verify";
    public static readonly string RefreshTokenUrl = BaseUrl + "/auth/refresh";

    public static readonly string GetAllCurrenciesUrl = BaseUrl + "/users/getAllCurrencies";
    public static readonly string SaveCurrenciesUrl = BaseUrl + "/users/saveCurrencies";

    public static readonly string GetAllStageStatusUrl = BaseUrl + "/users/progress/getAllStageStatus";
    public static readonly string SaveStageStatusUrl = BaseUrl + "/users/progress/saveStageStatus";

    public static readonly string GetInteriorByTheme = BaseUrl + "/users/progress/getInteriorByTheme";
    public static readonly string SaveInteriorUrl = BaseUrl + "/users/progress/saveSingleInterior";
    public static readonly string SaveInteriorsUrl = BaseUrl + "/users/progress/saveMultipleInterior";
    
    public static readonly string GetEmployeeByTheme = BaseUrl + "/users/progress/getEmployeeByTheme";
    public static readonly string SaveEmployeeUrl = BaseUrl + "/users/progress/saveSingleEmployee";
    public static readonly string SaveEmployeesUrl = BaseUrl + "/users/progress/saveMultipleEmployee";
    
    public static readonly string GetFoodByTheme = BaseUrl + "/users/progress/getFoodByTheme";
    public static readonly string SaveFoodUrl = BaseUrl + "/users/progress/saveSingleFood";
    public static readonly string SaveFoodsUrl = BaseUrl + "/users/progress/saveMultipleFood";
    
    public static readonly string InsertThemeRecordsUrl = BaseUrl + "/users/progress/insertRecords";
    public static readonly string GetThemeRecordsUrl = BaseUrl + "/users/progress/getRecords";
    public static readonly string GetRankingUrl = BaseUrl + "/users/progress/getRanking";
    public static readonly string SaveRankingUrl = BaseUrl + "/users/progress/saveRanking";
    public static readonly string GetRankPointUrl = BaseUrl + "/users/progress/getRankpoint";
    public static readonly string SaveRankPointUrl = BaseUrl + "/users/progress/saveRankpoint";
    public static readonly string GetCumulativeUrl = BaseUrl + "/users/progress/getCumulative";
    public static readonly string SaveCumulativeUrl = BaseUrl + "/users/progress/saveCumulative";
}

public static class Constants
{
    public static readonly float DEFAULT_ORDER_TIME = 0f;
    public static readonly int MAX_UPGRADE_LEVEL = 5;
    public static readonly float PLAYER_INTERACTION_SPEED = 3f;
    public static readonly int HEALTH_DECREASE_AMOUNT_ONWORKFINISHED = 2;
    public static readonly int HEALTH_DECREASE_AMOUNT_ONTIMEFINISHED = 1;
    public static readonly int DEFAULT_SINKINGSTATION_CAPACITY = 5;
}

public static class Variables
{
    private static DebugFlags debugFlags = DebugFlags.WorkSystem;

    public static DebugFlags DebugFlags
    {
        get => debugFlags;
        set
        {
            if (debugFlags != value) // 값이 변경되었을 때만 이벤트 발생
            {
                debugFlags = value;
                OnDebugFlagsChanged?.Invoke(debugFlags); // 이벤트 호출
            }
        }
    }

    public static event Action<DebugFlags> OnDebugFlagsChanged;
}

public enum CookwareType
{
    CoffeeMachine,
    DrinkingFountain,
    SparklingWaterMaker,
    Blender,
    Oven,
    SushiCountertop,
    Fryer,
    CharcoalGrill,
    GriddleGrill,
    Pot,
    KitchenTable,
    Refrigerator
}

public enum InteriorCategory
{
    테이블,
    카운터,
    인테리어,
    조리대,
    싱크대
}

public enum InteriorEffectType
{
    None,
    EatSpeedIncrease,
    TipProbabilityIncrease,
    RankPoints,
    CapacityIncrease,
    CraftTimeDecrease
}

public enum ObjectArea
{
    Hall = 1,
    Kitchen
}

public static class Strings
{
    public static readonly string PlayerTag = "Player";
    public static readonly string GameManagerTag = "GameManager";
    public static readonly string InteractableObjectTag = "InteractableObject";

    public static readonly string WorkDurationRatioSO = "WorkDurationRatioSO";

    public static readonly string CounterName = "Counter";
    public static readonly string FoodPickupCounterName = "FoodPickupCounter";
    public static readonly string CookingStation = "CookingStation";
    public static readonly string Table = "Table";
    public static readonly string Tray = "Tray";
    public static readonly string TrayReturnCounter = "TrayReturnCounter";
    public static readonly string SinkingStation = "SinkingStation";

    public static readonly string Bubble = "Bubble";
    public static readonly string Clean = "Clean";
    public static readonly string WashDish = "WashDish";
    public static readonly string Cash = "Cash";
}

public enum StringTableIds
{
    UIString
}