using System.Collections.Generic;
using HutongGames.PlayMaker;
using Mono.Posix;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer.CacheObject.Views;
using UnityExplorer.Inspectors;
using UniverseLib.UI;
using UniverseLib.UI.Widgets.ScrollView;

public class EventsPage(FsmInspector owner) : ICellPoolDataSource<FsmEventCell>
{
    public FsmInspector Ownner { get; set; } = owner;
    public PlayMakerFSM Target { get; set; } = owner?.Target;
    public List<FsmEvent> fsmEvents = new List<FsmEvent>();
    public int ItemCount => Target == null ? 0 : Target.FsmEvents.Length;
    public ScrollPool<FsmEventCell> events;
    public GameObject UIRoot { get; set; }
    public void OnCellBorrowed(FsmEventCell cell)
    {

    }
    public static EventsPage Create(GameObject parent, FsmInspector owner)
    {
        EventsPage page = new EventsPage(owner);
        page.UIRoot = UIFactory.CreateVerticalGroup(parent, "EventsPage", false, false, true, true);
        UIFactory.SetLayoutElement(page.UIRoot, flexibleHeight: 9999, flexibleWidth: 9999);

        page.events = UIFactory.CreateScrollPool<FsmEventCell>(page.UIRoot, "Events", out GameObject events_ui_root, out GameObject content);
        UIFactory.SetLayoutElement(events_ui_root, flexibleHeight: 9999, flexibleWidth: 9999);
        page.events.Initialize(page);

        return page;
    }
    public void SetTarget(PlayMakerFSM fsm)
    {
        Target = fsm;
        events.Refresh(true, true);
    }

    public void SetCell(FsmEventCell cell, int index)
    {
        if (Target == null) return;
        if (index < 0 || index >= ItemCount)
        {
            cell.Disable();
            return;
        }
        cell.Enable();
        cell.SetTarget(Target.FsmEvents[index]);
    }
    public void Update()
    {

    }
}
public class FsmEventCell : ICell
{
    public FsmEvent Target { get; set; }
    public bool Enabled => UIRoot.activeSelf;

    public RectTransform Rect { get; set; }
    public GameObject UIRoot { get; set; }

    public float DefaultHeight => 25f;
    public Text EventName { get; set; }
    public Toggle isGlobal;
    public Toggle isSystem;

    public GameObject CreateContent(GameObject parent)
    {
        UIRoot = UIFactory.CreateHorizontalGroup(parent, "EventCell", false, false, true, true);
        UIFactory.SetLayoutElement(UIRoot, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);
        Rect = UIRoot.GetComponent<RectTransform>();

        EventName = UIFactory.CreateLabel(UIRoot, "EventName", "NotSet", TextAnchor.MiddleLeft, Color.green);
        UIFactory.SetLayoutElement(EventName.gameObject, flexibleHeight: 9999, flexibleWidth: 9999);

        var global_toggle = UIFactory.CreateToggle(UIRoot, "GlobalToggle", out isGlobal, out Text global_toggle_text);
        global_toggle_text.text = "GLOBAL";
        isGlobal.interactable = false;
        UIFactory.SetLayoutElement(global_toggle, minWidth: 80, flexibleWidth: 0);

        var system_toggle = UIFactory.CreateToggle(UIRoot, "GlobalToggle", out isSystem, out Text system_toggle_text);
        system_toggle_text.text = "SYSTEM";
        isSystem.interactable = false;
        UIFactory.SetLayoutElement(system_toggle, minWidth: 80, flexibleWidth: 0);

        return UIRoot;
    }
    public void SetTarget(FsmEvent fsmEvent)
    {
        Target = fsmEvent;
        EventName.text = fsmEvent.Name;
        isGlobal.isOn = fsmEvent.IsGlobal;
        isSystem.isOn = fsmEvent.isSystemEvent;
    }

    public void Disable()
    {
        UIRoot.SetActive(false);
    }

    public void Enable()
    {
        UIRoot.SetActive(true);
    }
}