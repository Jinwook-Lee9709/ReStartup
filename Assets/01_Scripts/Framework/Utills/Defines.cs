using System;
using System.Collections.Generic;
using UnityEngine;

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
    Gold,
    AdTicket
}

public enum ResponseType
{
    Success = 0,
    Fail = 1,
    InvalidToken = 2,
    Timeout = 3,
    InternetDisconnected = 4,
    ServerError = 5,
    JsonParseError = 6,

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
    RankCondition,
    Mission,
    ThemeCondition,
    GuideCategory,
    GuideElement,
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
    Korean,
    English,
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

public enum MissionType
{
    Achievements,
    Main,
    Daily,
    Weekly,
}

public enum RequirementsType
{
    SoldFood,
    MakingFood
}
public enum RewardType
{
    Money,
    Gold,
    AdBlockTicket,
    MissionPoint,
    RankPoint
}
public enum MissionMainCategory
{
    BuyInterior=1,
    UnlockFood,
    SellingFood,
    UpgradeFood,
    HireStaff,
    Promotion,
    UpgradeInterior,
    GuestSatisfied,
    GainRanking,
    UpgradeStaff,
    CleanTrash,
    CleanTable,
    Recover,
    GainMoney,
    GetTip,
    GoodReview,
    BadReviewDelete
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

    public static readonly string GetUserName = BaseUrl + "/users/getName";
    public static readonly string SaveUserName = BaseUrl + "/users/updateName";

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
    public static readonly string SaveIsClaimedUrl = BaseUrl + "/users/progress/saveIsClaimed";
    

    public static readonly string GetPromotionsUrl = BaseUrl + "/users/getPromotions";
    public static readonly string SavePromotionsUrl = BaseUrl + "/users/savePromotions";

    public static readonly string GetBuffsUrl = BaseUrl + "/users/progress/getBuffs";
    public static readonly string SaveBuffUrl = BaseUrl + "/users/progress/saveBuff";
    public static readonly string SaveBuffsUrl = BaseUrl + "/users/progress/saveBuffs";
    public static readonly string DeleteBuffUrl = BaseUrl + "/users/progress/deleteBuff";

    public static readonly string GetAllReivewUrl = BaseUrl + "/users/progress/getAll";
    public static readonly string InsertReivewUrl = BaseUrl + "/users/progress/insert";
    public static readonly string DeleteReivewUrl = BaseUrl + "/users/progress/delete";

    public static readonly string GetRankerUrl = BaseUrl + "/users/general/getRanker";
    public static readonly string GetUserRankUrl = BaseUrl + "/users/general/getUserRank";
    
    public static readonly string GetMissionsUrl = BaseUrl + "/users/progress/getMissions";
    public static readonly string SaveMissionUrl = BaseUrl + "/users/progress/saveMission";
}

public static class Constants
{
    public static readonly float DEFAULT_ORDER_TIME = 0f;
    public static readonly int MAX_UPGRADE_LEVEL = 5;
    public static readonly float PLAYER_INTERACTION_SPEED = 3f;
    public static readonly int HEALTH_DECREASE_AMOUNT_ONWORKFINISHED = 2;
    public static readonly int HEALTH_DECREASE_AMOUNT_ONTIMEFINISHED = 1;
    public static readonly int DEFAULT_SINKINGSTATION_CAPACITY = 5;

    public static readonly int BUFF_SAVE_INTERVAL = 30;
    public static readonly int EMPLOYEE_SAVE_INTERVAL = 30;

    public static readonly float POP_UP_DURATION = 1f;
    
    public static readonly int MAX_SUPERVISOR_COUNT = 3;
    public static readonly int SUPERVISOR_HIRE_REQUIREMENTS = 15;
    public static readonly int MAX_RANK = 15;

    public static readonly int TRASH_CREATE_STACK = 20;
    public static readonly int TRASH_CLEAN_BONUS = 10;
}

public enum RankRewardType
{
    None,
    Money,
    Gold,
    InflowRate,
    AdDelete
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
    KitchenTable
}

public enum InteriorCategory
{
    Table,
    Counter,
    Interior,
    Cookware,
    Sink,
    TrashCan,
    Wallpaper,
    Floor,
    Decor,
}

public enum InteriorUICategory
{
    Furniture,
    Decor,
    BuiltInFurniture,
    KitchenEquipment
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
    None = 0,
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
    public static readonly string TrashCan = "TrashCan";
    public static readonly string TrashIcon = "TrashIcon";

    public static readonly string Bubble = "Bubble";
    public static readonly string Clean = "Clean";
    public static readonly string WashDish = "WashDish";
    public static readonly string Cash = "Cash";
    public static readonly string Gold = "Gold";
    public static readonly string Golds = "Golds";
    public static readonly string Free = "Free";
    public static readonly string Pay = "PayIcon";

    public static readonly string Check = "Check";
    public static readonly string Upgrade = "Upgrade";


    public static readonly string employeeNameKeyFormat = "EmployeeName{0}";
    public static readonly string cookwareFormat = "S{0}_{1}";
    public static readonly string complete = "Completed";

    public static readonly string foodNameKeyFormat = "FoodName{0}";
    public static readonly string shortOnMoney = "ShortOnMoney";

    public static readonly string buffNameFormat = "Buff{0}Name";
    public static readonly string buffDescriptionFormat = "Buff{0}Description";

    public static readonly string promotionNameFormat = "Promotion{0}Name";
    public static readonly string promotionDescriptionFormat = "Promotion{0}Description";

    public static readonly string buffCountFormat = "+{0} Buff";

    public static readonly string costFormat = "#,##0";
    public static readonly string cntFormat = "{0} / {1}";

    public static readonly string badWordTableFileName = "badwords";

    public static readonly string orderTextFormat = "{0}{1}order";
    public static readonly string servingDelayTextFormat = "{0}{1}servingdelay";
    public static readonly string paidverygoodTextFormat = "{0}{1}paidverygood";
    public static readonly string badTextFormat = "{0}{1}bad";

    public static readonly string positiveReviewFormat = "{0}1{1}";
    public static readonly string negativeReviewFormat = "{0}0{1}";

    public static readonly string randomReviewIDFormat = "{0}**{1}";
    public static readonly string alphaNums = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM0123456789";
    
    public static readonly string CompensationSoKey = "SupervisorCompensationSO";

    public static readonly string HallWallTileIdFormat = "T{0}HallWall{1}";
    public static readonly string HallFloorTileIdFormat = "T{0}HallFloor{1}";
    public static readonly string KitchenWallTileIdFormat = "T{0}KitchenWall{1}";
    public static readonly string KitchenFloorTileIdFormat = "T{0}KitchenFloor{1}";
    public static readonly string DecorTileIdFormat = "T{0}ThemeDecoration";
    
    public static readonly string EmblemIdFormat = "Emblem{0}";

    public static readonly string tutorialCompeleteKey = "TUTORIAL_ORDER_COMPLETE";
    public static readonly string tutorialPopupFormat = "TutorialPopup{0}";
    public static readonly string nameWarningFormat = "NameWarning{0}";

    public static readonly string positiveReviewFileName = "PositiveReview";
    public static readonly string negativeReviewFileName = "NegativeReview";

    public static readonly string promotionIconFormat = "Promotion{0}";

    public static readonly string adTicketPopupId = "AdTicketPopup";
}

public static class Colors
{
    public static List<Color> satisfactionColors = new()
    {
        new Color(Mathf.InverseLerp(0, 255, 202), Mathf.InverseLerp(0, 255, 235), Mathf.InverseLerp(0, 255, 255)),
        new Color(Mathf.InverseLerp(0, 255, 255), Mathf.InverseLerp(0, 255, 249), Mathf.InverseLerp(0, 255, 159)),
        new Color(Mathf.InverseLerp(0, 255, 255), Mathf.InverseLerp(0, 255, 128), Mathf.InverseLerp(0, 255, 125))
    };
    public static Color rankPanelPlayerColor = new(1, 0.9855481f, 0.6462264f);
    public static Color invisibleBlack = new Color(0, 0, 0, 0);
    public static Color invisibleWhite = new Color(1, 1, 1, 0);

}

public enum StringTableIds
{
    UIString
}