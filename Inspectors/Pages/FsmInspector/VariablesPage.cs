using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityExplorer.CacheObject;
using UnityExplorer.CacheObject.Views;
using UnityExplorer.Inspectors;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Widgets.ScrollView;

public class VariablesPage(FsmInspector owner) : ICellPoolDataSource<CacheMemberCell>
{
    public FsmInspector Owner { get; set; } = owner;
    public GameObject UIRoot { get; set; }
    public FsmVariables Target { get; set; }

    public int ItemCount => cacheMembers.Count;

    ScrollPool<CacheMemberCell> cells;
    List<CacheMember> cacheMembers = new();

    public enum VariableType
    {
        Array,
        Bool,
        Color,
        Enum,
        Float,
        GameObject,
        Int,
        Material,
        Object,
        Quaternion,
        Rect,
        String,
        Texture,
        Vector2,
        Vector3
    }


    public static VariablesPage Create(GameObject parent, FsmInspector owner)
    {
        var page = new VariablesPage(owner);
        page.UIRoot = UIFactory.CreateVerticalGroup(parent, "VariablesPage", false, false, true, true, bgColor: Color.clear, childAlignment: TextAnchor.UpperCenter);
        UIFactory.SetLayoutElement(page.UIRoot, flexibleWidth: 9999, flexibleHeight: 9999);
        page.cells = UIFactory.CreateScrollPool<CacheMemberCell>(page.UIRoot, "Variables", out GameObject cell_ui_root, out GameObject content);
        UIFactory.SetLayoutElement(cell_ui_root, flexibleHeight: 9999, flexibleWidth: 9999);
        page.cells.Initialize(page);
        return page;
    }
    public void SetTarget(FsmVariables variables)
    {
        Target = variables;
        cacheMembers.Clear();
        foreach (VariableType var_type in Enum.GetValues(typeof(VariableType)))
        {
            var pi = typeof(FsmVariables).GetProperty(var_type.ToString() + "Variables");
            if (pi != null)
            {
                var array = pi.GetValue(variables) as Array;
                if (array.Length > 0)
                {
                    cacheMembers.Add(new NICacheProperty(pi, variables));
                }
            }
        }
        cells.Refresh(true, true);

    }

    public void OnCellBorrowed(CacheMemberCell cell)
    {

    }
    public void Update()
    {
        foreach (var cell in cells.CellPool)
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

    public void SetCell(CacheMemberCell cell, int index)
    {
        if (index < 0 || index >= ItemCount)
        {
            cell.Disable();
            return;
        }
        cell.Enable();
        CacheObjectControllerHelper.SetCell(cell, index, cacheMembers, (cell) =>
        {
            // cell.SubContentHolder.SetActive(true);
            // cell.RefreshSubcontentButton();
        });

    }
}