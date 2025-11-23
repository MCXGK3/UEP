using UEP;

public static class HelperFun
{
    public static void LogInfo(this object msg)
    {
        UEPPlugin.logger.LogInfo(msg);
    }
}