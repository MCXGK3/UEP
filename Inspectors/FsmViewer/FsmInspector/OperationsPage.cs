using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer.CacheObject;
using UnityExplorer.CacheObject.Views;
using UnityExplorer.Inspectors;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.ObjectPool;

public class OperationsPage
{
    public GameObject UIRoot { get; set; }
    public FsmInspector Owner { get; set; }

    public GameObject ScrollView { get; set; }
    public GameObject ContentGroup { get; set; }
    public OperationBlock SelectedTransition { get; set; }
    public OperationBlock SelectedState { get; set; }
    public OperationBlock Method { get; set; }
    public OperationBlock Record { get; set; }

    public static OperationsPage Create(GameObject parent, FsmInspector Owner)
    {
        var page = new OperationsPage();
        page.Owner = Owner;
        page.UIRoot = UIFactory.CreateVerticalGroup(parent, "OperationsPage", false, false, true, true, bgColor: Color.clear, childAlignment: TextAnchor.UpperCenter);
        UIFactory.SetLayoutElement(page.UIRoot, flexibleHeight: 9999, flexibleWidth: 9999);

        page.ScrollView = UIFactory.CreateScrollView(page.UIRoot, "ScrollView", out GameObject content, out var scrollbar);
        UIFactory.SetLayoutElement(page.ScrollView, flexibleHeight: 9999, flexibleWidth: 9999);

        page.ContentGroup = UIFactory.CreateVerticalGroup(content, "ContentGroup", false, false, true, true, bgColor: Color.clear, childAlignment: TextAnchor.UpperCenter);
        UIFactory.SetLayoutElement(page.ContentGroup, flexibleHeight: 9999, flexibleWidth: 9999);

        page.SelectedTransition = new OperationBlock(page, page.ContentGroup, OperationBlock.OperationBlockType.SelectedTransition);
        page.SelectedState = new OperationBlock(page, page.ContentGroup, OperationBlock.OperationBlockType.SelectedState);
        page.Method = new OperationBlock(page, page.ContentGroup, OperationBlock.OperationBlockType.Method);
        page.Record = new OperationBlock(page, page.ContentGroup, OperationBlock.OperationBlockType.Record);

        return page;
    }
    public void Refresh()
    {
        SelectedState.Refresh();
        SelectedTransition.Refresh();
        Method.Refresh();
        Record.Refresh();
    }
    public void ClearAll()
    {
        SelectedState.ClearAll();
        SelectedTransition.ClearAll();
        Method.ClearAll();
        Record.ClearAll();
    }
}
public class OperationBlock
{
    public enum OperationBlockType
    {
        SelectedTransition,
        SelectedState,
        Method,
        Record
    }
    public RectTransform Rect { get; set; }
    public OperationBlockType Type { get; set; }
    public OperationsPage Owner { get; set; }
    public GameObject UIRoot { get; set; }
    public Text Title { get; set; }
    public GameObject NameRow { get; set; }
    public GameObject SubContentHolder { get; set; }
    public ButtonRef SubContentButton { get; set; }
    public readonly Color subInactiveColor = new(0.23f, 0.23f, 0.23f);
    public readonly Color subActiveColor = new(0.23f, 0.33f, 0.23f);
    Toggle record_toggle;
    Toggle mark_toggle;
    ButtonRef clear_button;

    public OperationBlock(OperationsPage owner, GameObject parent, OperationBlockType type)
    {
        Owner = owner;
        Type = type;
        UIRoot = UIFactory.CreateVerticalGroup(parent, "StateActionCell", false, false, true, true, childAlignment: TextAnchor.UpperCenter);
        UIFactory.SetLayoutElement(UIRoot.gameObject, minWidth: 300, flexibleWidth: 9999);
        Rect = UIRoot.GetComponent<RectTransform>();
        var ui_root_cf = UIRoot.AddComponent<ContentSizeFitter>();
        ui_root_cf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        NameRow = UIFactory.CreateHorizontalGroup(UIRoot, "NameRow",
                                                    false, false, true, true,
                                                    padding: new Vector4(0, 0, 10, 0),
                                                    bgColor: new Color(0, 1, 1, 0.7f), childAlignment: TextAnchor.MiddleLeft);
        UIFactory.SetLayoutElement(NameRow, 200, flexibleWidth: 9999, minHeight: 30, flexibleHeight: 0);

        SubContentButton = UIFactory.CreateButton(NameRow, "SubContentButton", "▲", subInactiveColor);
        UIFactory.SetLayoutElement(SubContentButton.Component.gameObject, minWidth: 25, minHeight: 25, flexibleWidth: 0, flexibleHeight: 0);
        SubContentButton.OnClick += SubContentClicked;


        Title = UIFactory.CreateLabel(NameRow, "ActionName", "notset", TextAnchor.MiddleLeft, Color.black);
        UIFactory.SetLayoutElement(Title.gameObject, minWidth: 150, flexibleWidth: 9999, flexibleHeight: 9999);
        Title.text = Type.ToString();

        var toggle_go = UIFactory.CreateToggle(NameRow, "RecordToggle", out record_toggle, out Text toggle_text);
        toggle_text.text = "Recording";
        record_toggle.isOn = false;
        UIFactory.SetLayoutElement(toggle_go, minWidth: 80, flexibleHeight: 9999, flexibleWidth: 0);
        toggle_go.SetActive(type == OperationBlockType.Record);

        toggle_go = UIFactory.CreateToggle(NameRow, "MarkToggle", out mark_toggle, out Text mark_toggle_text);
        mark_toggle_text.text = "Mark";
        mark_toggle.isOn = false;
        UIFactory.SetLayoutElement(toggle_go, minWidth: 80, flexibleHeight: 9999, flexibleWidth: 0);
        toggle_go.SetActive(type == OperationBlockType.Record);

        clear_button = UIFactory.CreateButton(NameRow, "ClearButton", "Clear");
        UIFactory.SetLayoutElement(clear_button.GameObject, minWidth: 80, preferredHeight: 25, flexibleWidth: 0);
        clear_button.OnClick += ClearButtonClicked;
        clear_button.GameObject.SetActive(type == OperationBlockType.Record);



        // var inspect_button = UIFactory.CreateButton(name, "InspectButton", "Inspect");
        // UIFactory.SetLayoutElement(inspect_button.GameObject, minWidth: 80, flexibleWidth: 0, flexibleHeight: 9999);
        SubContentHolder = UIFactory.CreateVerticalGroup(UIRoot, "SubContentHolder", false, false, true, true);
        UIFactory.SetLayoutElement(SubContentHolder, minWidth: 300, flexibleWidth: 9999);
        RefreshSubcontentButton();

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
    public void Update()
    {

    }
    public void Refresh()
    {
        switch (Type)
        {
            case OperationBlockType.Method:
                ClearMethods();
                CreateMethods();
                break;
            case OperationBlockType.Record:
                record_toggle.onValueChanged.RemoveListener(OnRecordToggleChange);
                record_toggle.onValueChanged.AddListener(OnRecordToggleChange);
                break;
        }
    }
    public void ClearAll()
    {
        switch (Type)
        {
            case OperationBlockType.SelectedTransition:
                ClearSelect();
                break;
            case OperationBlockType.SelectedState:
                ClearSelect();
                break;
            case OperationBlockType.Method:
                ClearMethods();
                break;
            case OperationBlockType.Record:
                StopRecord();
                record_toggle.onValueChanged.RemoveListener(OnRecordToggleChange);
                ClearButtonClicked();
                break;
        }
    }
    #region ForSelect
    List<ButtonLabel> labels = new();
    public void ClearSelect()
    {
        foreach (ButtonLabel label in labels)
        {
            label.OnReturnToPool();
        }
        labels.Clear();
    }
    public void Select(FsmTransition transition)
    {
        if (Type != OperationBlockType.SelectedTransition) return;
        ClearSelect();
        string text = FsmUtils.ParseFsmTransition(transition);
        Action action = transition.toFsmState == null ? null : () => Owner.Owner.SelectState(transition.toState);
        string button_text = "Select";
        var label = ButtonLabel.OnBorredFromPool(SubContentHolder, text, action, button_text);
        labels.Add(label);

    }
    public void Select(FsmState state)
    {
        if (Type != OperationBlockType.SelectedState) return;
        if (state == null) return;
        ClearSelect();
        Title.text = "SelectedState: " + "<color=green>" + state.Name + "</color>";
        foreach (var transition in state.transitions)
        {
            if (transition == null) continue;
            string text = FsmUtils.ParseFsmTransition(transition);
            Action action = transition.toFsmState == null ? null : () => Owner.Owner.SelectLine(transition);
            string button_text = "Select";
            var label = ButtonLabel.OnBorredFromPool(SubContentHolder, text, action, button_text);
            labels.Add(label);
        }
    }

    #endregion

    #region ForMethod

    List<CacheMemberCell> method_cells = new List<CacheMemberCell>();
    public List<CacheMember> methods = new();
    public void ClearMethods()
    {
        foreach (var cell in method_cells)
        {
            if (cell == null) continue;
            cell.Disable();
            Pool<CacheMemberCell>.Return(cell);
        }
        method_cells.Clear();
        methods.Clear();
    }
    public void GetMethodInfos()
    {
        var send_event_info = typeof(PlayMakerFSM).GetMethod("SendEvent");
        methods.Add(new NICacheMethod(send_event_info, Owner.Owner.Target));
        var set_state_info = typeof(PlayMakerFSM).GetMethod("SetState");
        methods.Add(new NICacheMethod(set_state_info, Owner.Owner.Target));
    }
    public void CreateMethods()
    {
        if (Owner.Owner.Target == null) return;
        GetMethodInfos();
        for (int i = 0; i < methods.Count; i++)
        {
            var cell = Pool<CacheMemberCell>.Borrow();
            cell.Rect.SetParent(SubContentHolder.transform, false);
            method_cells.Add(cell);
            CacheObjectControllerHelper.SetCell(cell, i, methods, null);
            cell.Enable();

        }

    }


    #endregion

    #region ForRecord
    public bool IsRecording => record_toggle != null && record_toggle.isOn;
    public bool AddMark => mark_toggle != null && mark_toggle.isOn;
    List<ButtonLabel> record_labels = new List<ButtonLabel>();
    private void OnRecordToggleChange(bool val)
    {
        if (val)
        {
            FsmWatcher.Register(Owner.Owner.Target, this);
        }
        else
        {
            FsmWatcher.Unregister(Owner.Owner.Target);
        }

    }
    private void StopRecord()
    {
        mark_toggle.isOn = false;
        record_toggle.isOn = false;
    }
    private void ClearButtonClicked()
    {
        foreach (var label in record_labels)
        {
            label.OnReturnToPool();
        }
        record_labels.Clear();
        Owner.Owner.ClearAllMarks();
    }
    public void AddEventRecord(bool is_global, string from_state_name, string event_name, string to_state_name)
    {
        string text = "";
        if (is_global)
        {
            text = "<color=grey>Global: </color>" + event_name + "<color=grey>--></color>" + "<color=green>" + to_state_name + "</color>";
        }
        else
        {
            text = "<color=green>" + from_state_name + "</color><color=grey>--</color>" + event_name + "<color=grey>--></color>" + "<color=green>" + to_state_name + "</color>";
        }

        var bl = ButtonLabel.OnBorredFromPool(SubContentHolder,
        text,
        () => Owner.Owner.SelectLine(is_global, from_state_name, event_name),
        "Select"
        );
        if (AddMark)
        {
            Owner.Owner.MarkLine(is_global, from_state_name, event_name);
            Owner.Owner.MarkState(to_state_name);
        }
        record_labels.Add(bl);
    }
    public void AddSetStateRecord(string state_name)
    {
        var bl = ButtonLabel.OnBorredFromPool(SubContentHolder,
        "<color=grey>SetState: </color>" + "<color=green>" + state_name + "</color>",
        () => Owner.Owner.SelectState(state_name),
        "Select"
        );
        if (AddMark)
        {
            Owner.Owner.MarkState(state_name);
        }
        record_labels.Add(bl);
    }
    public void AddErrorRecord(string msg)
    {
        var bl = ButtonLabel.OnBorredFromPool(SubContentHolder, "Error: " + msg);
        bl.Label.color = Color.red;
        record_labels.Add(bl);
    }



    #endregion

}