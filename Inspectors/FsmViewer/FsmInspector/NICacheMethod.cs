using System;
using System.Reflection;
using UnityExplorer.CacheObject;
using UniverseLib;
using UniverseLib.Utility;

public class NICacheMethod : CacheMethod
{
    public object Target { get; set; }
    public new object DeclaringInstance => Target.TryCast(DeclaringType);
    public NICacheMethod(MethodInfo mi, object target) : base(mi)
    {
        Target = target;
        base.SetInspectorOwner(null, mi);
    }
    public override object TryEvaluate()
    {
        try
        {
            MethodInfo methodInfo = MethodInfo;
            if (methodInfo.IsGenericMethod)
            {
                methodInfo = MethodInfo.MakeGenericMethod(base.Evaluator.TryParseGenericArguments());
            }

            object result = ((!HasArguments) ? methodInfo.Invoke(DeclaringInstance, ArgumentUtility.EmptyArgs) : methodInfo.Invoke(DeclaringInstance, base.Evaluator.TryParseArguments()));
            base.LastException = null;
            return result;
        }
        catch (Exception lastException)
        {
            base.LastException = lastException;
            return null;
        }
    }
}