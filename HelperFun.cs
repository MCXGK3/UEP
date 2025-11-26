using UEP;

public static class HelperFun
{
    public static void LogInfo(this object msg)
    {
        UEPPlugin.logger.LogInfo(msg);
    }
    public static void LogWarning(this object msg)
    {
        UEPPlugin.logger.LogWarning(msg);
    }
}