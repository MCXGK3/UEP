using System;
using System.Reflection;
using UnityExplorer.CacheObject;
using UniverseLib;
using UniverseLib.Utility;

public class NICacheField : CacheField
{
    public NICacheField(FieldInfo fi, object target) : base(fi)
    {
        NameLabelText = "<color=" + SignatureHighlighter.keywordBlueHex + ">" + fi.Name + "</color>";
        NameForFiltering = SignatureHighlighter.RemoveHighlighting(NameLabelText);
        NameLabelTextRaw = NameForFiltering;
        Target = target;
    }
    public object Target { get; set; }
    public new object DeclaringInstance => IsStatic ? null : Target.TryCast(DeclaringType);


    protected override object TryEvaluate()
    {
        try
        {
            object ret = FieldInfo.GetValue(DeclaringInstance);
            LastException = null;
            return ret;
        }
        catch (Exception ex)
        {
            LastException = ex;
            return null;
        }
    }
    protected override void TrySetValue(object value)
    {
        try
        {
            FieldInfo.SetValue(DeclaringInstance, value);
        }
        catch (Exception ex)
        {
            ex.LogInfo();
        }
    }
}