using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UEP;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExplorer.UI.Panels;
using UnityExplorer.UI.Widgets;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.ObjectPool;
using UniverseLib.UI.Widgets;
using UniverseLib.UI.Widgets.ScrollView;
using UniverseLib.Utility;
namespace UnityExplorer.Inspectors;

public class FsmInspector : InspectorBase
{
    public new PlayMakerFSM Target => (PlayMakerFSM)base.Target;

    public GameObject content;

    public GameObjectControls controls;

    public TransformTree transformTree;

    Text fsm_name_text;
    InputFieldRef hidden_name_text;
    Toggle fsm_started;
    Toggle use_template;
    string tab_button_text;
    string info_page_state;
    ButtonRef state_tab;
    GameObject state_page;
    ButtonRef events_tab;
    GameObject events_page;
    ButtonRef variables_tab;
    GameObject variables_page;

    GameObject fsm_view_content;
    GameObject fsm_view;

    Text fsm_view_pos;
    Text fsm_active_state_name;
    Text fsm_selected_state_name;

    const float min_fsm_view_scale = 0.2f;
    const float max_fsm_view_scale = 3f;

    public float scaleSpeed = 0.1f;
    public float scaleLerpSpeed = 8f;
    Vector3 current_fsm_view_scale => fsm_view_content.transform.localScale;

    List<FsmNode> fsmNodes = new List<FsmNode>();

    FsmNode Selected_FsmState { get; set; }
    Dictionary<string, FsmNode> node_dicts = new();
    FsmNode Active_FsmState { get; set; }
    ScrollPool<StateActionCell> actions_info;
    StateActionList action_info_list = new(null);
    Toggle auto_refresh_toggle;
    public bool AutoRefresh => auto_refresh_toggle.isOn;

    Toggle info_view_toggle;

    GameObject info_view;



    public override void OnBorrowedFromPool(object target)
    {
        base.OnBorrowedFromPool(target);
        SetTarget(Target);

        UEPPlugin.Instance.StartCoroutine(InitCoroutine());
    }
    private IEnumerator InitCoroutine()
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(InspectorPanel.Instance.ContentRect);
    }
    public override void CloseInspector()
    {
        InspectorManager.ReleaseInspector(this);
    }

    public override GameObject CreateContent(GameObject parent)
    {

        UIRoot = UIFactory.CreateVerticalGroup(parent, "FsmInspector", false, false, true, true, 5,
                new Vector4(4, 4, 4, 4), new Color(0.065f, 0.065f, 0.065f));
        #region TopRow
        var top_row = UIFactory.CreateHorizontalGroup(UIRoot, "toprow", false, false, true, true, 5,
            default, new(0.1f, 0.1f, 0.1f), TextAnchor.MiddleLeft);
        UIFactory.SetLayoutElement(top_row, minHeight: 35, flexibleHeight: 0, flexibleWidth: 9999);
        GameObject titleHolder = UIFactory.CreateUIObject("TitleHolder", top_row);
        UIFactory.SetLayoutElement(titleHolder, minHeight: 35, flexibleHeight: 0, flexibleWidth: 9999);

        fsm_name_text = UIFactory.CreateLabel(titleHolder, "VisibleTitle", "NotSet", TextAnchor.MiddleLeft);
        RectTransform namerect = fsm_name_text.GetComponent<RectTransform>();
        namerect.anchorMin = new Vector2(0, 0);
        namerect.anchorMax = new Vector2(1, 1);
        fsm_name_text.fontSize = 17;
        UIFactory.SetLayoutElement(fsm_name_text.gameObject, minHeight: 35, flexibleHeight: 0, minWidth: 300, flexibleWidth: 9999);

        hidden_name_text = UIFactory.CreateInputField(titleHolder, "Title", "not set");
        RectTransform hiddenrect = hidden_name_text.Component.gameObject.GetComponent<RectTransform>();
        hiddenrect.anchorMin = new Vector2(0, 0);
        hiddenrect.anchorMax = new Vector2(1, 1);
        hidden_name_text.Component.readOnly = true;
        hidden_name_text.Component.lineType = InputField.LineType.MultiLineNewline;
        hidden_name_text.Component.gameObject.GetComponent<Image>().color = Color.clear;
        hidden_name_text.Component.textComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
        hidden_name_text.Component.textComponent.fontSize = 17;
        hidden_name_text.Component.textComponent.color = Color.clear;
        UIFactory.SetLayoutElement(hidden_name_text.Component.gameObject, minHeight: 35, flexibleHeight: 0, flexibleWidth: 9999);


        var toggle_go = UIFactory.CreateToggle(top_row, "InfoViewToggle", out info_view_toggle, out Text info_view_toggle_text);
        info_view_toggle.isOn = true;
        info_view_toggle_text.text = "Open Info";
        UIFactory.SetLayoutElement(toggle_go, minWidth: 100, flexibleWidth: 0);
        info_view_toggle.onValueChanged.AddListener((val) =>
        {
            info_view.SetActive(val);
        });



        #endregion

        #region StateRow
        GameObject state_row = UIFactory.CreateHorizontalGroup(UIRoot, "state_row", false, false, true, true, 5, default, new(0.05f, 0.05f, 0.05f, 1), childAlignment: TextAnchor.MiddleLeft);
        UIFactory.SetLayoutElement(state_row, minHeight: 25, flexibleHeight: 0, flexibleWidth: 9999);

        var started = UIFactory.CreateToggle(state_row, "started", out fsm_started, out Text text);
        fsm_started.interactable = false;
        text.text = "Started";
        UIFactory.SetLayoutElement(started, minWidth: 100, flexibleWidth: 0);

        var useTemplate = UIFactory.CreateToggle(state_row, "usesTemplate", out use_template, out text);
        use_template.interactable = false;
        text.text = "UsesTemplate";
        UIFactory.SetLayoutElement(useTemplate, minWidth: 100, flexibleWidth: 0);

        fsm_view_pos = UIFactory.CreateLabel(state_row, "FsmViewPos", "not set", TextAnchor.MiddleCenter);
        UIFactory.SetLayoutElement(fsm_view_pos.gameObject, minWidth: 100, flexibleWidth: 0);

        var fsm_view_pos_reset = UIFactory.CreateButton(state_row, "FsmViewPosReset", "Reset");
        UIFactory.SetLayoutElement(fsm_view_pos_reset.GameObject, minWidth: 100, minHeight: 25, flexibleWidth: 0);
        fsm_view_pos_reset.OnClick = () =>
        {
            fsm_view_content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        };

        fsm_active_state_name = UIFactory.CreateLabel(state_row, "FsmActiveStateName", "not set", TextAnchor.MiddleCenter);
        UIFactory.SetLayoutElement(fsm_active_state_name.gameObject, minWidth: 100, flexibleWidth: 0);
        var fsm_active_state_locate = UIFactory.CreateButton(state_row, "FsmActiveStateLocate", "Locate");
        UIFactory.SetLayoutElement(fsm_active_state_locate.GameObject, minWidth: 100, minHeight: 25, flexibleWidth: 0);
        fsm_active_state_locate.OnClick += () =>
        {
            if (Target.ActiveStateName != null)
            {
                node_dicts.TryGetValue(Target.ActiveStateName, out FsmNode node);
                if (node != null)
                {
                    fsm_view_content.GetComponent<RectTransform>().anchoredPosition = -node.RectTransform.anchoredPosition;
                }
            }

        };

        fsm_selected_state_name = UIFactory.CreateLabel(state_row, "FsmSelectedStateName",
        "<color=grey>Selected: </color>null",
        TextAnchor.MiddleCenter);
        UIFactory.SetLayoutElement(fsm_selected_state_name.gameObject, minWidth: 100, flexibleWidth: 0);
        var fsm_selected_state_locate = UIFactory.CreateButton(state_row, "FsmActiveStateLocate", "Locate");
        UIFactory.SetLayoutElement(fsm_selected_state_locate.GameObject, minWidth: 100, minHeight: 25, flexibleWidth: 0);
        fsm_selected_state_locate.OnClick += () =>
        {
            if (Selected_FsmState != null)
            {
                fsm_view_content.GetComponent<RectTransform>().anchoredPosition = -Selected_FsmState.RectTransform.anchoredPosition;
            }

        };

        #endregion

        #region Content
        var content = UIFactory.CreateHorizontalGroup(UIRoot, "Content", false, false, true, true, 5, default, new(0, 0, 0), TextAnchor.MiddleLeft);
        UIFactory.SetLayoutElement(content, minWidth: 300, flexibleWidth: 9999, flexibleHeight: 9999);

        fsm_view = UIFactory.CreateScrollView(content, "FsmView", out fsm_view_content, out AutoSliderScrollbar scrollbar);
        UIFactory.SetLayoutElement(fsm_view, minWidth: 0, flexibleWidth: 9999, flexibleHeight: 9999);
        CreateFsmView(fsm_view, fsm_view_content, scrollbar);

        info_view = UIFactory.CreateVerticalGroup(content, "InfoView", false, false, true, true, 5, default, new(0.1f, 0.1f, 0.1f, 0.5f), TextAnchor.UpperCenter);
        UIFactory.SetLayoutElement(info_view, minWidth: 400, preferredWidth: 600, flexibleHeight: 9999, flexibleWidth: 0);

        var info_tabs_holder = UIFactory.CreateHorizontalGroup(info_view, "InfoTabHolder", false, false, true, true, 5, default, new(1, 1, 1, 0), TextAnchor.MiddleLeft);
        UIFactory.SetLayoutElement(info_tabs_holder, minHeight: 35, flexibleHeight: 0, flexibleWidth: 9999);

        var info_tab_color = new ColorBlock()
        {
            normalColor = Color.grey,
            selectedColor = Color.blue,
            pressedColor = Color.black,
            highlightedColor = Color.blue,
            disabledColor = new Color(0.1f, 0.1f, 0.1f),
        };
        state_tab = UIFactory.CreateButton(info_tabs_holder, "StateTab", "State", info_tab_color);
        UIFactory.SetLayoutElement(state_tab.GameObject, minWidth: 90, minHeight: 30, flexibleHeight: 0, flexibleWidth: 0);

        events_tab = UIFactory.CreateButton(info_tabs_holder, "EventTab", "Event", info_tab_color);
        UIFactory.SetLayoutElement(events_tab.GameObject, minWidth: 90, minHeight: 30, flexibleHeight: 0, flexibleWidth: 0);

        variables_tab = UIFactory.CreateButton(info_tabs_holder, "VariablesTab", "Variables", info_tab_color);
        UIFactory.SetLayoutElement(variables_tab.GameObject, minWidth: 90, minHeight: 30, flexibleHeight: 0, flexibleWidth: 0);

        var refresh_toggle = UIFactory.CreateToggle(info_tabs_holder, "AutoRefreshToggle", out auto_refresh_toggle, out Text refresh_text);
        auto_refresh_toggle.isOn = false;
        refresh_text.text = "Auto-update";
        UIFactory.SetLayoutElement(refresh_toggle, minWidth: 100, flexibleWidth: 0);

        var info_details = UIFactory.CreateHorizontalGroup(info_view, "InfoDetails", false, false, true, true, bgColor: Color.clear, childAlignment: TextAnchor.UpperCenter);
        UIFactory.SetLayoutElement(info_details, flexibleHeight: 9999, flexibleWidth: 9999);

        state_page = UIFactory.CreateVerticalGroup(info_details, "StatePage", false, false, true, true, bgColor: Color.clear, childAlignment: TextAnchor.UpperCenter);
        UIFactory.SetLayoutElement(state_page, flexibleHeight: 9999, flexibleWidth: 9999);
        actions_info = UIFactory.CreateScrollPool<StateActionCell>(state_page, "StateActions",
        out GameObject actions_ui_root, out GameObject actions_content);
        UIFactory.SetLayoutElement(actions_ui_root, flexibleHeight: 9999, flexibleWidth: 9999);
        actions_info.Initialize(action_info_list);
        state_page.SetActive(false);

        events_page = UIFactory.CreateVerticalGroup(info_details, "EventsPage", false, false, true, true, bgColor: Color.yellow, childAlignment: TextAnchor.UpperCenter);
        UIFactory.SetLayoutElement(events_page, flexibleHeight: 9999, flexibleWidth: 9999);
        events_page.SetActive(false);

        variables_page = UIFactory.CreateVerticalGroup(info_details, "VariablesPage", false, false, true, true, bgColor: Color.red, childAlignment: TextAnchor.UpperCenter);
        UIFactory.SetLayoutElement(variables_page, flexibleHeight: 9999, flexibleWidth: 9999);
        variables_page.SetActive(false);

        state_tab.OnClick += () => ChangeInfoPage("State");

        events_tab.OnClick += () => ChangeInfoPage("Events");

        variables_tab.OnClick += () => ChangeInfoPage("Variables");
        #endregion

        return UIRoot;
    }

    private void ChangeInfoPage(string page_name)
    {
        bool is_state = false;
        bool is_event = false;
        bool is_variables = false;
        switch (page_name)
        {
            case "State":
                info_page_state = page_name;
                is_state = true;
                break;
            case "Events":
                info_page_state = page_name;
                is_event = true;
                break;
            case "Variables":
                info_page_state = page_name;
                is_variables = true;
                break;
            default:
                return;
        }
        state_tab.Component.interactable = !is_state;
        state_page.SetActive(is_state);
        events_tab.Component.interactable = !is_event;
        events_page.SetActive(is_event);
        variables_tab.Component.interactable = !is_variables;
        variables_page.SetActive(is_variables);
        return;
    }

    public override void Update()
    {
        if (!IsActive)
        {
            return;
        }
        if (base.Target.IsNullOrDestroyed(false))
        {
            InspectorManager.ReleaseInspector(this);
            return;
        }
        fsm_view_pos.text = "<color=grey>View Pos: </color>" + (-fsm_view_content.GetComponent<RectTransform>().anchoredPosition).ToString("F6");
        fsm_active_state_name.text = "<color=grey>Active: </color>" + Target.ActiveStateName;
        if (Target.ActiveStateName != null && node_dicts.TryGetValue(Target.ActiveStateName, out FsmNode node))
        {
            if (node != Active_FsmState)
            {
                Active_FsmState?.FsmInActive();
                node.FsmActive();
                Active_FsmState = node;
            }
        }
        if (AutoRefresh)
        {
            switch (info_page_state)
            {
                case "State":
                    foreach (var state_action_cell in actions_info.CellPool)
                    {
                        if (!state_action_cell.Enabled || state_action_cell.Target == null)
                            continue;
                        state_action_cell.Update();
                    }
                    break;
                default: break;
            }
        }
    }
    Vector2 GetMousePositionInContent()
    {
        var viewport = fsm_view.GetComponent<ScrollRect>().viewport;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, Input.mousePosition, null, out localPoint);
        return localPoint;
    }
    public void ZoomAtPoint(float scaleDelta, Vector2 zoomCenter)
    {
        var viewport = fsm_view.GetComponent<ScrollRect>().viewport;
        float targetScale = current_fsm_view_scale.x;
        // 计算新的缩放比例
        float newScale = targetScale + scaleDelta;
        newScale = Mathf.Clamp(newScale, min_fsm_view_scale, max_fsm_view_scale);

        // 如果缩放比例没有变化，直接返回
        if (Mathf.Approximately(newScale, targetScale))
            return;

        // 计算缩放中心在content局部空间中的位置
        Vector2 contentPivot = fsm_view_content.GetComponent<RectTransform>().pivot;
        Vector2 viewportCenter = viewport.rect.center;
        Vector2 localZoomCenter = zoomCenter;

        // 计算缩放前后的位置变化
        Vector2 contentPosBefore = fsm_view_content.transform.localPosition;
        float scaleRatio = newScale / targetScale;

        // 应用新的缩放比例
        targetScale = newScale;

        // 计算新的内容位置，保持缩放中心不变
        Vector2 offset = localZoomCenter - (Vector2)fsm_view_content.transform.localPosition;
        Vector2 contentPosAfter = localZoomCenter - offset * scaleRatio;

        fsm_view_content.transform.localPosition = contentPosAfter;
        fsm_view_content.transform.localScale = Vector3.one * targetScale;


    }


    private void HandleZoomInput(Vector2 scroll_data)
    {
        float scroll = scroll_data.y;

        if (scroll != 0 && IsMouseOverViewport())
        {
            ZoomAtPoint(scroll * scaleSpeed, GetMousePositionInContent());
        }
    }

    bool IsMouseOverViewport()
    {
        var viewport = fsm_view.GetComponent<ScrollRect>().viewport;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(viewport, Input.mousePosition, null, out localPoint);
        return viewport.rect.Contains(localPoint);
    }

    private void CreateFsmView(GameObject scroll_view, GameObject content, AutoSliderScrollbar autoSlider)
    {
        scroll_view.AddComponent<CheckScroll>().returnScrollData += HandleZoomInput;
        ScrollRect scroll_rect = scroll_view.GetComponent<ScrollRect>();
        scroll_rect.horizontal = true;
        scroll_rect.movementType = ScrollRect.MovementType.Unrestricted;
        scroll_rect.vertical = true;
        scroll_rect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
        scroll_rect.scrollSensitivity = 0;
        scroll_rect.inertia = false;
        var layoutgroup = content.GetComponent<VerticalLayoutGroup>();
        if (layoutgroup != null)
        {
            GameObject.DestroyImmediate(layoutgroup);
        }
        autoSlider.UIRoot.SetActive(false);

        var size_fitter = content.GetComponent<ContentSizeFitter>();
        if (size_fitter != null)
        {
            size_fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            size_fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        RectTransform content_rt = content.GetComponent<RectTransform>();
        content_rt.anchorMin = new Vector2(0.5f, 0.5f);
        content_rt.anchorMax = new Vector2(0.5f, 0.5f);
        content_rt.pivot = new Vector2(0.5f, 0.5f);

        UIFactory.SetLayoutElement(content, preferredHeight: 500, preferredWidth: 500);



        return;

    }

    private void CreateNode(Vector2 pos, string name, List<string> events = null)
    {
        var node = Pool<FsmNode>.Borrow();
        node.UIRoot.transform.SetParent(fsm_view_content.transform, false);
        node.SetTarget(pos, name, events);
        fsmNodes.Add(node);
    }
    private void CreateNode(FsmState fsmState, bool is_begin_state = false)
    {
        var node = Pool<FsmNode>.Borrow();
        node.UIRoot.transform.SetParent(fsm_view_content.transform, false);
        node.SetTarget(this, fsmState, is_begin_state);
        fsmNodes.Add(node);
        node_dicts.Add(fsmState.Name, node);
    }

    public override void OnReturnToPool()
    {
        ClearAll();
        info_view_toggle.isOn = true;
        auto_refresh_toggle.isOn = false;
        base.OnReturnToPool();

    }
    public void ClearAll()
    {
        foreach (var node in fsmNodes)
        {
            node.OnReturnToPool();
        }
        fsmNodes.Clear();
        node_dicts.Clear();
        fsm_name_text.text = "notset";
        fsm_started.isOn = true;
        use_template.isOn = true;
        fsm_selected_state_name.text = "<color=grey>Selected: </color>null";


    }

    private void SetTarget(PlayMakerFSM target)
    {
        tab_button_text = "<color=yellow>" + target.FsmName + "</color>" + "<color=white>(</color>" + "<color=green>" + target.name + "</color>" + "<color=white>)</color>";
        Tab.TabText.text = tab_button_text;
        fsm_name_text.text = "<color=grey>FSM Name: </color>" + "<color=yellow>" + target.FsmName + "</color>";
        hidden_name_text.Text = SignatureHighlighter.RemoveHighlighting(fsm_name_text.text);
        fsm_started.isOn = target.fsm.Started;
        use_template.isOn = target.UsesTemplate;
        string begin_state_name = Target.fsm.startState;
        foreach (var state in Target.FsmStates)
        {
            CreateNode(state, state.Name == begin_state_name);
        }
    }
    public void SelectState(FsmNode node)
    {
        if (node == null)
        {
            return;
        }
        if (Selected_FsmState != null)
        {
            Selected_FsmState.UnSelect();
        }
        node.Select();
        Selected_FsmState = node;
        fsm_selected_state_name.text = "<color=grey>Selected: </color>" + node.Target.Name;
        action_info_list.SetTarget(node);
        actions_info.Refresh(true, true);
    }
}