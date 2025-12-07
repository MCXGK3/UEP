using System;
using System.Drawing;
using System.Reflection;
using UnityExplorer.CacheObject;
using UniverseLib;
using UniverseLib.Utility;

public class NICacheProperty : CacheProperty
{
    public NICacheProperty(PropertyInfo pi, object target) : base(pi)
    {
        NameLabelText = "<color=" + SignatureHighlighter.keywordBlueHex + ">" + pi.Name + "</color>";
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
            object ret;
            if (HasArguments)
                ret = PropertyInfo.GetValue(DeclaringInstance, this.Evaluator.TryParseArguments());
            else
                ret = PropertyInfo.GetValue(DeclaringInstance, null);
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
        if (!CanWrite)
            return;

        try
        {
            bool _static = PropertyInfo.GetAccessors(true)[0].IsStatic;

            if (HasArguments)
                PropertyInfo.SetValue(DeclaringInstance, value, Evaluator.TryParseArguments());
            else
                PropertyInfo.SetValue(DeclaringInstance, value, null);
        }
        catch (Exception ex)
        {
            (ex).LogInfo();
        }
    }

}