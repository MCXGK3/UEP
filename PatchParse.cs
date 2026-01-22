using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using HutongGames.PlayMaker;
using UnityEngine;
using UniverseLib;
namespace UEP;

internal static class PatchParse
{
    public delegate object TParseMethod(string input);
    public delegate string TToStringMethod(object obj);
    public delegate string TComponentToStringMethod(object obj, string originalString);

    public static object customTypesToString_obj;
    public static object customTypes_obj;
    public static IDictionary customTypesToString;
    public static IDictionary customTypes;
    public static Dictionary<Type, TToStringMethod> customToStrings = new Dictionary<Type, TToStringMethod>();
    public static Dictionary<Type, TComponentToStringMethod> customToStringsForComponent = new();

    public static TypeInfo parse_type;
    public static TypeInfo to_string_type;
    public static HashSet<string> types = new();
    static System.MulticastDelegate d;
    static PatchParse()
    {
        HackParseUtility();
    }

    public static void RegisterToString(Type type, TToStringMethod method)
    {
        if (type == null || method == null) return;
        if (!customToStrings.ContainsKey(type))
            customToStrings.Add(type, method);
    }
    public static void RegisterToString<T>(TToStringMethod method)
    {
        var type = typeof(T);
        if (type == null || method == null) return;
        if (!customToStrings.ContainsKey(type))
            customToStrings.Add(type, method);
    }
    public static void Register(Type type, TParseMethod parseMethod, TToStringMethod toStringMethod)
    {
        types.Add(type.FullName);
        customTypes[type.FullName] = Delegate.CreateDelegate(parse_type, parseMethod.Method);
        customTypesToString[type.FullName] = Delegate.CreateDelegate(to_string_type, toStringMethod.Method);
    }
    public static void RegisterComponent<T>(TComponentToStringMethod toStringMethod) where T : Component
    {
        RegisterComponent(typeof(T), toStringMethod);
    }
    public static void RegisterComponent(Type type, TComponentToStringMethod toStringMethod)
    {
        if (typeof(Component).IsAssignableFrom(type) == false) return;
        if (customToStringsForComponent.ContainsKey(type)) return;
        customToStringsForComponent.Add(type, toStringMethod);
    }
    public static void HackParseUtility()
    {
        UEP.UEPPlugin.logger.LogInfo("Patching ParseUtility");
        customTypes_obj = Traverse.Create(typeof(UniverseLib.Utility.ParseUtility)).Field("customTypes").GetValue();
        parse_type = Traverse.Create(typeof(UniverseLib.Utility.ParseUtility)).Type("ParseMethod").GetValue<TypeInfo>();
        customTypes = (IDictionary)customTypes_obj;
        customTypesToString_obj = Traverse.Create(typeof(UniverseLib.Utility.ParseUtility)).Field("customTypesToString").GetValue();
        to_string_type = Traverse.Create(typeof(UniverseLib.Utility.ParseUtility)).Type("ToStringMethod").GetValue<TypeInfo>();
        customTypesToString = (IDictionary)customTypesToString_obj;
        UEP.UEPPlugin.logger.LogInfo("Patching OK");
    }

}
[HarmonyPatch(typeof(UnityExplorer.CacheObject.CacheObjectBase), "SetDataToCell")]
public static class PatchSetDataToCell
{
    public static void Postfix(UnityExplorer.CacheObject.CacheObjectBase __instance, UnityExplorer.CacheObject.Views.CacheObjectCell cell)
    {
        if (__instance.Value?.GetType()?.FullName == null) return;
        if (PatchParse.types.Contains(__instance.Value?.GetType()?.FullName))
        {
            bool canWrite = __instance.CanWrite;
            bool canWrite2 = __instance.CanWrite;
            Traverse.Create(__instance).Method("SetValueState", cell, new UnityExplorer.CacheObject.CacheObjectBase.ValueStateArgs(
                true, true, (UnityEngine.Color?)null, false, false, canWrite, canWrite2, true, false
            )
            ).GetValue();
        }
    }
}
[HarmonyPatch(typeof(UniverseLib.Utility.ToStringUtility), "ToStringWithType")]
public static class PatchToStringWithType
{
    public static bool Prefix(ref string __result, object value, System.Type fallbackType, bool includeNamespace)
    {
        if (value?.GetType() == null) return true;
        foreach (var kv in PatchParse.customToStrings)
        {
            if (kv.Key.IsAssignableFrom(value?.GetType()) == true)
            {
                var res = PatchParse.customToStrings[kv.Key](value);
                if (res == null) return true;
                __result = res;
                return false;
            }
        }

        return true;
    }

}
[HarmonyPatch(typeof(UnityExplorer.UI.Widgets.ComponentList), "SetComponentCell")]
public static class PathchSetComponentCell
{
    public static Dictionary<Type, PropertyInfo> enabledProp = new Dictionary<Type, PropertyInfo>();
    public static void Postfix(UnityExplorer.UI.Widgets.ComponentList __instance, UnityExplorer.UI.Widgets.ComponentCell cell, int index)
    {
        Component data = Traverse.Create(__instance.Parent).Method("GetComponentEntries").GetValue<List<Component>>()[index];
        Type type = data.GetType();
        if (PatchParse.customToStringsForComponent.TryGetValue(type, out var toStringFunc))
        {
            string extra = toStringFunc(data, cell.Button.ButtonText.text);
            if (!string.IsNullOrEmpty(extra))
            {
                cell.Button.ButtonText.text = extra;
            }
        }
        if (data is not MonoBehaviour)
        {
            PropertyInfo enabled;
            if (!enabledProp.TryGetValue(type, out enabled))
            {
                enabled = Enumerable.FirstOrDefault<PropertyInfo>(type.GetProperties(), (PropertyInfo x) => x.Name.Equals("enabled", StringComparison.OrdinalIgnoreCase) && x.PropertyType == typeof(bool) && x.CanWrite && x.CanRead);
                enabledProp[type] = enabled;
            }
            if (enabled != null)
            {
                bool e = (bool)enabled.GetValue(data);
                cell.BehaviourToggle.interactable = true;
                cell.BehaviourToggle.SetIsOnWithoutNotify(e);
                cell.BehaviourToggle.graphic.color = new UnityEngine.Color(0.8f, 1f, 0.8f, 0.3f);
            }
        }
    }
}

[HarmonyPatch(typeof(UnityExplorer.UI.Widgets.ComponentList), "OnBehaviourToggled", typeof(bool), typeof(int))]
public class Patch_ComponentList_OnBehaviourToggled
{
    public static bool Prefix(UnityExplorer.UI.Widgets.ComponentList __instance, bool value, int index)
    {
        return true;
    }
    public static void Postfix(UnityExplorer.UI.Widgets.ComponentList __instance, bool value, int index)
    {
        Component data = ((List<Component>)Traverse.Create(__instance.Parent).Method("GetComponentEntries").GetValue())[index];
        Type type = data.GetType();
        PropertyInfo p;
        bool flag = PathchSetComponentCell.enabledProp.TryGetValue(type, out p);
        if (flag)
        {
            p.SetValue(data, value);
        }
    }
}

