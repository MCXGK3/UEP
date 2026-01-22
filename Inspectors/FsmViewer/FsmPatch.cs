using System;
using System.Collections.Generic;
using HarmonyLib;
using HutongGames.PlayMaker;
using UEP;
using UnityEngine;
using UnityExplorer;
using UnityExplorer.CacheObject;
using UnityExplorer.Inspectors;
using UnityExplorer.UI.Panels;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.ObjectPool;
using UniverseLib.Utility;

[HarmonyPatch]
public class AddFsmInspector
{

}
// [HarmonyPatch(typeof(InspectorManager), "Inspect", [typeof(object), typeof(CacheObjectBase)])]
public class Patch_InspectorManager_Inspect
{
    public static bool Prefix(object obj, CacheObjectBase parent)
    {
        return true;
    }
    public static void Postfix(ref object obj, CacheObjectBase parent)
    {
        if (obj.IsNullOrDestroyed())
            return;

        obj = obj.TryCast();
        if (obj is Fsm fsm)
        {
            Traverse.CreateWithType("InspectorManager").Method("CreateInspector", [typeof(object)]).GetValue(fsm);
        }
    }
    public static void CreateFsmInspector(object target, bool staticReflection = false, CacheObjectBase parent = null)
    {
        var im = Traverse.CreateWithType("InspectorManager");
        var Inspectors = im.Field("Inspectors").GetValue<List<InspectorBase>>();
        bool TryFocusActiveFsmInspector(object target)
        {
            foreach (InspectorBase inspector in Inspectors)
            {
                bool shouldFocus = false;

                if (target is Type targetAsType)
                {
                    if (inspector.TargetType.FullName == targetAsType.FullName)
                        shouldFocus = true;
                }
                else if (inspector.Target.ReferenceEqual(target) && (inspector is FsmInspector))
                {
                    shouldFocus = true;
                }

                if (shouldFocus)
                {
                    UEUIManager.SetPanelActive(UEUIManager.Panels.Inspector, true);
                    InspectorManager.SetInspectorActive(inspector);
                    return true;
                }
            }
            return false;
        }
        if (TryFocusActiveFsmInspector(target))
        {
            return;
        }
        FsmInspector inspector = Pool<FsmInspector>.Borrow();

        Inspectors.Add(inspector);
        ((InspectorBase)inspector).Target = target;

        UEUIManager.SetPanelActive(UEUIManager.Panels.Inspector, true);
        inspector.UIRoot.transform.SetParent(InspectorPanel.Instance.ContentHolder.transform, false);

        inspector.OnBorrowedFromPool(target);
        InspectorManager.SetInspectorActive(inspector);

        im.Field("OnInspectedTabsChanged").GetValue<Action>()?.Invoke();
    }

}

[HarmonyPatch(typeof(ReflectionInspector), "CreateContent", MethodType.Normal)]
public class Patch_ReflectionInspector_CreateContent
{
    public static bool Prefix(ReflectionInspector __instance, GameObject parent)
    {
        return true;
    }
    public static void Postfix(ReflectionInspector __instance, GameObject parent)
    {
        var fsm_button = UIFactory.CreateButton(__instance.UIRoot.transform.Find("TopRow").gameObject, "Fsm Inspector", "Open Fsm Inspector", new Color(0.2f, 0.2f, 0.2f, 1));
        fsm_button.ButtonText.color = Color.white;
        UIFactory.SetLayoutElement(fsm_button.GameObject, minWidth: 140, flexibleWidth: 0, minHeight: 25, flexibleHeight: 0);
        fsm_button.OnClick += () =>
        {
            Patch_InspectorManager_Inspect.CreateFsmInspector(__instance.Target);
        };
        fsm_button.GameObject.SetActive(false);
    }
}
public class Patch_ReflectionInspector_For_Fsm
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ReflectionInspector), "SetTarget")]
    public static void Postfix(ReflectionInspector __instance, object target)
    {
        if (target is PlayMakerFSM)
        {
            __instance.UIRoot.transform.Find("TopRow/Fsm Inspector").gameObject.SetActive(true);
            UEPPlugin.logger.LogInfo("Open a fsm");
        }
        else
        {
            __instance.UIRoot.transform.Find("TopRow/Fsm Inspector").gameObject.SetActive(false);
        }
    }
}
