public enum DataTableIds
{
    Employee,
    Food
}

public enum ThemeIds
{
    Theme1,
    Theme2,
    Theme3
}


public enum WorkType
{
    Payment,
    Clean,
    Hall,
    Kitchen,
}

public enum WorkStatus
{
    Waiting,
    Assigned,
}

public enum InteractStatus
{
    Pending,
    Progressing,
    Success,
}
public enum LanguageType
{
    Korean,
}

public enum SceneIds
{
    Title,
    Lobby,
    Loading,
    Theme1,
    Theme2,
    Theme3,
    Dev1,
    Dev2,
    Dev3,
}

public enum BuffType
{
    Influencer,
}

public static class Endpoints
{
    private static string BaseUrl = "127.0.0.1:3000";
    public static string UserUrl = BaseUrl + "/users";
}

public static class Constants
{
    public static float DEFAULT_ORDER_TIME = 0f;
    public static int MAX_UPGRADE_LEVEL = 5;
}

public static class Strings
{
    public static readonly string GameManagerTag = "GameManager";
}
