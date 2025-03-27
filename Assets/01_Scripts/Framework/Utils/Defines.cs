public enum DataTableIds
{
    Employee,
    Food
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

public static class Endpoints
{
    private static string BaseUrl = "127.0.0.1:3000";
    public static string UserUrl = BaseUrl + "/users";
}

public static class Constants
{
    public static float defaultOrderTime = 0f;
}

