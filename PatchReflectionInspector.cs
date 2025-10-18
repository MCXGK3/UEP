using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;
using UnityExplorer.CacheObject;
using UnityExplorer.Inspectors;
using UnityExplorerPlus.Inspectors.Reflect;

internal static class PatchReflectionInspector
{
    public static List<PropertyInfo> autoEvalBlacklist = new List<PropertyInfo>
    {
        typeof(MeshFilter).GetProperty("mesh"),
        typeof(Renderer).GetProperty("material"),
        typeof(Renderer).GetProperty("materials"),
        typeof(Material).GetProperty("color"),
        typeof(Material).GetProperty("mainTexture"),
        typeof(Material).GetProperty("mainTextureOffset"),
        typeof(Material).GetProperty("mainTextureScale")
    };
    public static ConditionalWeakTable<Material, Shader> shader_cache = new ConditionalWeakTable<Material, Shader>();

    public static void AttachShaderCache(Material mat, ReflectionInspector self)
    {
        PatchReflectionInspector.shader_cache.Remove(mat);
        Shader shader = mat.shader;
        PatchReflectionInspector.shader_cache.Add(mat, shader);
        bool flag = shader == null;
        if (!flag)
        {
            List<CacheMember> list = Traverse.Create(self).Field("members").GetValue<List<CacheMember>>();
            List<CacheMember> i = new List<CacheMember>();
            for (int j = 0; j < shader.GetPropertyCount(); j++)
            {
                string name = shader.GetPropertyName(j);
                CacheShaderProp cm = new CacheShaderProp();
                cm.BindShaderProp(name);
                cm.SetInspectorOwner(self, null);
                i.Add(cm);
            }
            list.InsertRange(0, i);
        }
    }

    // Token: 0x0600011F RID: 287 RVA: 0x00008E3E File Offset: 0x0000703E
    public static void RemoveShaderCache(ReflectionInspector self)
    {
        Traverse.Create(self).Field("members").GetValue<List<CacheMember>>().RemoveAll((CacheMember x) => x is CacheShaderProp || x is CacheShaderKeywords);
    }

}
[HarmonyPatch(typeof(UnityExplorer.CacheObject.CacheProperty), "ShouldAutoEvaluate", MethodType.Getter)]
internal static class PatchShouldAutoEvaluate
{
    public static bool Prefix(UnityExplorer.CacheObject.CacheProperty __instance, ref bool __result)
    {
        if (PatchReflectionInspector.autoEvalBlacklist.Contains(__instance.PropertyInfo))
        {
            __result = false;
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(UnityExplorer.Inspectors.ReflectionInspector), "Update", MethodType.Normal)]
public static class Patch_UnityExplorer_Inspectors_ReflectionInspector_Update
{
    public static bool Prefix(UnityExplorer.Inspectors.ReflectionInspector __instance)
    {
        Material mat = __instance.Target as Material;
        bool flag = mat != null;
        if (flag)
        {
            Shader shader;
            bool flag2 = PatchReflectionInspector.shader_cache.TryGetValue(mat, out shader);
            if (flag2)
            {
                bool flag3 = mat.shader != shader;
                if (flag3)
                {
                    Traverse.Create(__instance).Field("refreshWanted").SetValue(true);
                    PatchReflectionInspector.RemoveShaderCache(__instance);
                    PatchReflectionInspector.AttachShaderCache(mat, __instance);
                }
            }
        }
        return true;
    }
    public static void Postfix(UnityExplorer.Inspectors.ReflectionInspector __instance)
    {
    }
}

[HarmonyPatch(typeof(ReflectionInspector), "SetTarget", new Type[] { typeof(object) })]
public static class Patch_ReflectionInspector_SetTarget
{
    public static bool Prefix(ReflectionInspector __instance, object target)
    {
        return true;
    }
    public static void Postfix(ReflectionInspector __instance, object target)
    {
        Material mat = target as Material;
        bool flag = mat != null;
        if (flag)
        {
            PatchReflectionInspector.AttachShaderCache(mat, __instance);
        }
    }
}