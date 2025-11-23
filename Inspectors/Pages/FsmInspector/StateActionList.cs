using UnityEngine.UIElements;
using UniverseLib.UI.Widgets.ScrollView;

public class StateActionList(FsmNode target) : ICellPoolDataSource<StateActionCell>
{
    public FsmNode Target = target;
    public int ItemCount => Target == null ? 0 : Target.Target.actions.GetCount();

    public bool AutoRefresh => Target.owner.AutoRefresh;

    public void OnCellBorrowed(StateActionCell cell)
    {

    }
    public void SetTarget(FsmNode target)
    {
        this.Target = target;
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

}