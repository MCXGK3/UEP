using System.Collections.Generic;
using HarmonyLib;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer.Inspectors;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.ObjectPool;
using UniverseLib.UI.Widgets.ScrollView;

public class FsmNode : IPooledObject
{
    public GameObject UIRoot { get; set; }
    public float DefaultHeight => -1f;

    public GameObject GameObject { get; set; }

    public RectTransform RectTransform { get; set; }

    public ButtonRef StateName { get; set; }

    public List<string> events_name = new();
    public Dictionary<FsmEvent, NodeEventCell> events_dict = new();

    GameObject event_ui_root;
    public FsmState Target { get; set; }
    public FsmInspector owner;
    public ColorBlock default_color;


    public GameObject CreateContent(GameObject parent)
    {
        UIRoot = UIFactory.CreateVerticalGroup(parent, "FsmNode", false, false, true, true, padding: new Vector4(2, 2, 2, 2), bgColor: Color.black, childAlignment: TextAnchor.UpperCenter);
        RectTransform = UIRoot.GetComponent<RectTransform>();
        var sf1 = UIRoot.AddComponent<ContentSizeFitter>();
        sf1.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        sf1.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var content = UIFactory.CreateHorizontalGroup(UIRoot, "Content", false, false, true, true, bgColor: new Color(0.1f, 0.1f, 0.1f, 0.5f), childAlignment: TextAnchor.UpperCenter);
        UIFactory.SetLayoutElement(content, minWidth: 100, minHeight: 16, flexibleHeight: 0, flexibleWidth: 9999);
        var content_sf = content.AddComponent<ContentSizeFitter>();
        content_sf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        content_sf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        content_sf.enabled = false;
        ColorBlock state_colors = new()
        {
            highlightedColor = Color.yellow,
            normalColor = Color.grey,
            disabledColor = Color.blue,
        };
        StateName = UIFactory.CreateButton(content, "StateName", "StateName", state_colors);
        default_color = StateName.Component.colors;
        StateName.ButtonText.horizontalOverflow = HorizontalWrapMode.Wrap;
        UIFactory.SetLayoutElement(StateName.GameObject, minWidth: 100, minHeight: 16, flexibleHeight: 0, flexibleWidth: 9999);
        StateName.GameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        StateName.GameObject.GetComponent<ContentSizeFitter>().enabled = false;
        UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(StateName.GameObject, padLeft: 5, padRight: 5);
        StateName.ButtonText.gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        StateName.OnClick += OnClickNode;
        // UIFactory.SetLayoutElement(state_name.ButtonText.gameObject, minWidth: 80, minHeight: 20, flexibleHeight: 0, flexibleWidth: 0);
        event_ui_root = UIFactory.CreateVerticalGroup(UIRoot, "StateEvents", false, false, true, true, 1, bgColor: new Color(1, 1, 1, 0.5f), childAlignment: TextAnchor.UpperCenter);
        UIFactory.SetLayoutElement(event_ui_root, minWidth: 100);
        event_ui_root.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        // event_cells = UIFactory.CreateScrollPool<NodeEventCell>(UIRoot, "StateEvents", out GameObject event_ui_root, out GameObject event_content, bgColor: Color.yellow);
        // scroll_layout = event_ui_root.GetComponent<LayoutElement>();
        // var sr = event_ui_root.GetComponent<ScrollRect>();
        // sr.scrollSensitivity = 0;
        UIFactory.SetLayoutElement(event_ui_root, minHeight: 0, minWidth: 100, flexibleWidth: 9999);

        return UIRoot;
    }

    public void OnCellBorrowed(NodeEventCell cell)
    {

    }
    public void Mark()
    {
        RuntimeHelper.SetColorBlock(StateName.Component, Color.cyan);
        StateName.ButtonText.color = Color.black;

    }
    public void UnMark()
    {
        RuntimeHelper.SetColorBlock(StateName.Component, default_color);
        StateName.ButtonText.color = Color.white;
    }


    public void ClearEvents()
    {
        foreach (var evt in events_dict.Values)
        {
            evt.OnReturnToPool();
        }
        events_dict.Clear();
    }
    private void RefreshEvents()
    {
        ClearEvents();

        foreach (var transition in Target.Transitions)
        {
            var evt = Pool<NodeEventCell>.Borrow();
            evt.UIRoot.transform.SetParent(event_ui_root.transform, false);
            evt.SetTarget(transition.FsmEvent, false, transition.toFsmState == null);
            events_dict.Add(transition.FsmEvent, evt);
        }

    }
    public void OnReturnToPool()
    {
        UnSelect();
        NotBeginState();
        UnMark();
        FsmInActive();
        ClearEvents();
        Target = null;
        this.owner = null;
        Pool<FsmNode>.Return(this);
    }
    public void OnClickNode()
    {
        owner.SelectState(this);
    }
    public void Select()
    {
        StateName.Component.interactable = false;
    }
    public void UnSelect()
    {
        StateName.Component.interactable = true;
    }
    public void FsmActive()
    {
        UIRoot.GetComponent<Image>().color = Color.white;
    }
    public void FsmInActive()
    {
        UIRoot.GetComponent<Image>().color = Color.black;
    }
    public void IsBeginState()
    {
        StateName.ButtonText.color = Color.yellow;
    }
    public void NotBeginState()
    {
        StateName.ButtonText.color = Color.white;
    }

    public void SetTarget(FsmInspector owner, FsmState fsmState, bool is_begin_state = false)
    {
        this.owner = owner;
        Vector2 pos = fsmState.position.center;
        RectTransform.anchoredPosition = new Vector2(pos.x, -pos.y);
        UIFactory.SetLayoutElement(UIRoot, minWidth: (int)fsmState.position.width);
        StateName.ButtonText.text = fsmState.Name;
        Target = fsmState;
        RefreshEvents();
        if (is_begin_state)
        {
            IsBeginState();
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);

    }
}