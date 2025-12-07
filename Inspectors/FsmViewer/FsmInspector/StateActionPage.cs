using UnityEngine;
using UnityEngine.UIElements;
using UnityExplorer.Inspectors;
using UniverseLib.UI;
using UniverseLib.UI.Widgets.ScrollView;

public class StateActionPage() : ICellPoolDataSource<StateActionCell>
{
    public StateNode Target;
    public FsmInspector Owner { get; set; }
    public GameObject UIRoot { get; set; }
    public ScrollPool<StateActionCell> cells;
    public int ItemCount => (Target == null || Target.Target == null || Target.Target.Actions == null) ? 0 : Target.Target.Actions.GetCount();

    public bool AutoRefresh => Target.owner.AutoRefresh;

    public void OnCellBorrowed(StateActionCell cell)
    {

    }
    public void SetTarget(StateNode target)
    {
        this.Target = target;
        cells.Refresh(true, true);
    }
    public static StateActionPage Create(GameObject parent, FsmInspector owner)
    {
        var page = new StateActionPage();
        page.UIRoot = UIFactory.CreateVerticalGroup(parent, "StateActionPage", false, false, true, true, bgColor: Color.clear, childAlignment: TextAnchor.UpperCenter);
        UIFactory.SetLayoutElement(page.UIRoot, flexibleWidth: 9999, flexibleHeight: 9999);
        page.cells = UIFactory.CreateScrollPool<StateActionCell>(page.UIRoot, "Actions", out GameObject cell_ui_root, out GameObject content);
        UIFactory.SetLayoutElement(cell_ui_root, flexibleHeight: 9999, flexibleWidth: 9999);
        page.cells.Initialize(page);
        return page;
    }
    public void ClearAll()
    {
        SetTarget(null);
    }
    public void SetCell(StateActionCell cell, int index)
    {
        if (index < 0 || index >= ItemCount)
        {
            cell.Disable();
            return;
        }
        cell.Enable();
        cell.SetTarget(Target.Target.actions[index], index);
        return;
    }
    public void Update()
    {
        foreach (var cell in cells.CellPool)
        {
            if (!cell.Enabled || cell.Target == null)
                continue;
            cell.Update();
        }
    }
}