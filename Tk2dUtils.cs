using UEP;
internal static class Tk2dUtils
{
    public static void Init()
    {
        // PatchParse.RegisterToString<tk2dSpriteAnimationClip>(Parsetk2dSpriteAnimationClip);
    }

    internal static string Parsetk2dSpriteAnimationClip(object clip)
    {
        tk2dSpriteAnimationClip c = (tk2dSpriteAnimationClip)clip;
        return string.Format("<color=grey>tk2dSpriteAnimationClip: </color><color=green>{0}</color><color=grey> (</color><color=green>{1}</color><color=grey>)</color>", c.name, c.wrapMode.ToString());
    }
}
