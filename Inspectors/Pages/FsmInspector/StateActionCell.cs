using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer;
using UnityExplorer.CacheObject;
using UnityExplorer.CacheObject.Views;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.ObjectPool;
using UniverseLib.UI.Widgets.ScrollView;

public class StateActionCell : ICell
{
    public bool Enabled => UIRoot.activeSelf;

    public RectTransform Rect { get; set; }
    public GameObject UIRoot { get; set; }

    public float DefaultHeight => 50f;
    public FsmStateAction Target { get; set; }
    public GameObject name_row;
    public Text action_name_text;

    public List<CacheMemberCell> parameter_cells = new();
    public List<CacheMember> parameters = new();
    public GameObject SubContentHolder { get; set; }
    public ButtonRef SubContentButton { get; set; }
    public readonly Color subInactiveColor = new(0.23f, 0.23f, 0.23f);
    public readonly Color subActiveColor = new(0.23f, 0.33f, 0.23f);

    public GameObject CreateContent(GameObject parent)
    {
        UIRoot = UIFactory.CreateVerticalGroup(parent, "StateActionCell", false, false, true, true, childAlignment: TextAnchor.UpperCenter);
        UIFactory.SetLayoutElement(UIRoot.gameObject, minWidth: 300, flexibleWidth: 9999);
        Rect = UIRoot.GetComponent<RectTransform>();
        var ui_root_cf = UIRoot.AddComponent<ContentSizeFitter>();
        ui_root_cf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        name_row = UIFactory.CreateHorizontalGroup(UIRoot, "NameRow",
                                                    false, false, true, true,
                                                    padding: new Vector4(0, 0, 10, 0),
                                                    bgColor: new Color(0, 1, 1), childAlignment: TextAnchor.MiddleLeft);
        UIFactory.SetLayoutElement(name_row, 200, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);

        SubContentButton = UIFactory.CreateButton(name_row, "SubContentButton", "▲", subInactiveColor);
        UIFactory.SetLayoutElement(SubContentButton.Component.gameObject, minWidth: 25, minHeight: 25, flexibleWidth: 0, flexibleHeight: 0);
        SubContentButton.OnClick += SubContentClicked;


        action_name_text = UIFactory.CreateLabel(name_row, "ActionName", "notset", TextAnchor.MiddleLeft, Color.black);
        UIFactory.SetLayoutElement(action_name_text.gameObject, minWidth: 150, flexibleWidth: 9999, flexibleHeight: 9999);
        var inspect_button = UIFactory.CreateButton(name_row, "InspectButton", "Inspect");
        UIFactory.SetLayoutElement(inspect_button.GameObject, minWidth: 80, flexibleWidth: 0, flexibleHeight: 9999);
        inspect_button.OnClick += () => InspectorManager.Inspect(Target);


        SubContentHolder = UIFactory.CreateVerticalGroup(UIRoot, "SubContentHolder", false, false, true, true);
        UIFactory.SetLayoutElement(SubContentHolder, minWidth: 300, flexibleWidth: 9999);


        return UIRoot;
    }
    public void RefreshSubcontentButton()
    {
        this.SubContentButton.ButtonText.text = SubContentHolder.activeSelf ? "▼" : "▲";
        Color color = SubContentHolder.activeSelf ? subActiveColor : subInactiveColor;
        color.a = 0.5f;
        RuntimeHelper.SetColorBlock(SubContentButton.Component, color, color * 1.3f);
    }
    private void SubContentClicked()
    {
        SubContentHolder.SetActive(!SubContentHolder.activeSelf);
        RefreshSubcontentButton();
    }

    public void SetTarget(FsmStateAction action, int index)
    {
        name_row.GetComponent<Image>().color = action.enabled ? new Color(0, 1, 1, 0.7f) : new Color(1, 0, 0, 0.7f);
        action_name_text.color = action.enabled ? Color.black : Color.white;
        action_name_text.text = " (" + index + ")" + " " + action.GetActualType().Name;
        Target = action;
        AddParameters();
        SubContentHolder.SetActive(true);
        RefreshSubcontentButton();
    }
    public void AddParameters()
    {
        ClearAll();
        parameters = GetCacheMemberList();
        for (int i = 0; i < parameters.Count; i++)
        {
            var cmc = Pool<CacheMemberCell>.Borrow();
            cmc.Rect.SetParent(SubContentHolder.GetComponent<RectTransform>(), false);
            parameter_cells.Add(cmc);
            CacheObjectControllerHelper.SetCell(cmc, i, parameters, null);
            cmc.Enable();
        }
    }
    public List<CacheMember> GetCacheMemberList()
    {
        void TryCacheProperty(List<CacheMember> props, Type type, PropertyInfo info)
        {
            try
            {
                if (!info.CanRead && info.CanWrite)
                {
                    return;
                }
                var cached = new NICacheProperty(info, Target);
                cached.SetFallbackType(info.PropertyType);
                props.Add(cached);

            }
            catch (Exception ex)
            {

            }
        }
        void TryCacheField(List<CacheMember> fields, Type type, FieldInfo info)
        {
            try
            {

                var cached = new NICacheField(info, Target);
                cached.SetFallbackType(info.FieldType);
                fields.Add(cached);
            }
            catch (Exception ex)
            {

            }
        }
        List<CacheMember> props = new();
        List<CacheMember> fields = new();
        Type type = Target.GetType();
        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance;
        foreach (var prop in type.GetProperties(flags))
        {
            if (prop.DeclaringType == type)
            {
                TryCacheProperty(props, type, prop);
            }
        }
        foreach (var field in type.GetFields(flags))
        {
            if (field.DeclaringType == type)
            {
                TryCacheField(props, type, field);
            }
        }
        return props.Concat(fields).ToList();

    }

    public void ClearAll()
    {
        foreach (var cell in parameter_cells)
        {
            cell.Disable();
            Pool<CacheMemberCell>.Return(cell);
        }
        parameter_cells.Clear();
        parameters.Clear();
    }
    public void Disable()
    {
        UIRoot?.SetActive(false);
    }

    public void Enable()
    {
        UIRoot?.SetActive(true);
    }

    public void Update()
    {
        if (SubContentHolder.activeSelf)
        {
            // "Update1".LogInfo();
            foreach (var cell in parameter_cells)
            {
                // (cell.Occupant.NameLabelTextRaw + " Update2 " + cell.Enabled).LogInfo();
                if (!cell.Enabled || cell.Occupant == null)
                    continue;
                CacheMember member = cell.MemberOccupant;
                // (member.NameForFiltering + "Update3").LogInfo();
                if (member.ShouldAutoEvaluate)
                {
                    // "Update".LogInfo();
                    member.Evaluate();
                    member.SetDataToCell(member.CellView);
                }
            }
        }
    }
}