using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Mono.Posix;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer;
using UnityExplorer.Inspectors;
using UnityExplorer.Inspectors.MouseInspectors;
using UnityExplorer.UI.Panels;
using UnityExplorerPlus;
using UnityExplorerPlus.Inspectors;
using UniverseLib.UI;

namespace UEP;

// TODO - adjust the plugin guid as needed
[BepInDependency("com.sinai.unityexplorer", BepInDependency.DependencyFlags.HardDependency)]
[BepInAutoPlugin(id: "io.github.shownyoung.uep")]
public partial class UEPPlugin : BaseUnityPlugin
{
    public static ManualLogSource logger;
    public static Dictionary<int, MouseInspectorBase> inspectors = new Dictionary<int, MouseInspectorBase>();
    public static Harmony harmony;
    public static UEPPlugin Instance { get; private set; }
    private void Awake()
    {
        logger = Logger;
        // Put your initialization logic here
        Logger.LogInfo($"Plugin {Name} ({Id}) has loaded!");
        harmony = new Harmony(Id);
        StartCoroutine(Init());
        Instance = this;
    }
    private IEnumerator Init()
    {
        harmony.PatchAll();
        while (UEUIManager.Initializing)
        {
            yield return null;
        }
        InitPanel();
        AddInspector("Collider2Ds", new Collider2DsInspector());
        AddInspector("Enemy", new EnemyInspector());
        AddInspector("World Position", new WorldPositionPin());
        // harmony.PatchAll(typeof(Patch_MouseInspector_OnDropdownSelect));
        // harmony.PatchAll(typeof(Patch_MouseInspector_CurrentInspector));
        FsmUtils.Init();
        Tk2dUtils.Init();

    }
    public static void AddInspector(string name, MouseInspectorBase inspector)
    {
        int id = InspectorPanel.Instance.MouseInspectDropdown.options.Count;
        InspectorPanel.Instance.MouseInspectDropdown.options.Add(new Dropdown.OptionData(name));
        inspectors.Add(id, inspector);
    }


    public void InitPanel()
    {

        UIBase uibase = Traverse.CreateWithType("UnityExplorer.UI.UIManager").Property("UiBase").GetValue<UIBase>();
        Dictionary<UEUIManager.Panels, UEPanel> UIPanels = Traverse.CreateWithType("UnityExplorer.UI.UIManager").Field("UIPanels").GetValue<Dictionary<UEUIManager.Panels, UEPanel>>();
        CustomPanel[] panels = new CustomPanel[]
        {
                // new ModPanel(uibase)
        };
        foreach (CustomPanel v in panels)
        {
            UIPanels.Add(v.PanelType, v);
            try
            {
                UEUIManager.SetPanelActive(v, false);
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }
    }

}
[HarmonyPatch(typeof(MouseInspector), "OnDropdownSelect", typeof(int))]
public class Patch_MouseInspector_OnDropdownSelect
{
    public static bool Prefix(MouseInspector __instance, int index)
    {
        MouseInspectorBase mouseInspectorBase;
        bool flag = UEPPlugin.inspectors.TryGetValue(index, out mouseInspectorBase);
        if (flag)
        {
            InspectorPanel.Instance.MouseInspectDropdown.value = 0;
            MouseInspector.Instance.StartInspect((MouseInspectMode)index);
            return false;
        }
        else
        {
            return true;
        }
    }
    public static void Postfix(MouseInspector __instance, int index)
    {
    }
}
[HarmonyPatch(typeof(MouseInspector), "CurrentInspector", MethodType.Getter)]
public class Patch_MouseInspector_CurrentInspector
{
    public static bool Prefix(MouseInspector __instance, ref MouseInspectorBase __result)
    {

        MouseInspectorBase insp;
        bool flag = UEPPlugin.inspectors.TryGetValue((int)MouseInspector.Mode, out insp);
        if (flag)
        {
            __result = insp;
            return false;
        }
        else
        {
            return true;
        }
    }
    public static void Postfix(MouseInspector __instance, MouseInspectorBase __result)
    {
    }
}