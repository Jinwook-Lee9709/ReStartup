using System;

[Flags]
public enum DebugFlags
{
    None = 0,
    FrameRate = 1 << 0,
    WorkSystem = 1 << 1,
}

public enum DataTableIds
{
    Employee,
    Food,
    Consumer,
    Ranking
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
    Title,
    Lobby,
    Loading,
    Theme1,
    Theme2,
    Theme3,
    Dev0,
    Dev1,
    Dev2,
    Dev3
}

public enum BuffType
{
    Influencer
}

public static class Endpoints
{
    private static readonly string BaseUrl = "127.0.0.1:3000";
    public static string UserUrl = BaseUrl + "/users";
}

public static class Constants
{
    public static readonly float DEFAULT_ORDER_TIME = 0f;
    public static readonly int MAX_UPGRADE_LEVEL = 5;
    public static readonly float PLAYER_INTERACTION_SPEED = 3f;
}

public static class Variables
{
    private static DebugFlags debugFlags = DebugFlags.WorkSystem;
    public static event Action<DebugFlags> OnDebugFlagsChanged;
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
    Griddle,
    KitchenTable,
}

public enum ObjectArea
{
    Hall,
    Kitchen,
}

public static class Strings
{
    public static readonly string PlayerTag = "Player";
    public static readonly string GameManagerTag = "GameManager";
    public static readonly string InteractableObjectTag = "InteractableObject";
    
    public static readonly string CounterName = "Counter";
    public static readonly string FoodPickupCounterName = "FoodPickupCounter";
    public static readonly string CookingStation = "CookingStation";
    public static readonly string Table = "Table";
}