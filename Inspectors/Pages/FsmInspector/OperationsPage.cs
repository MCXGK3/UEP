using UnityEngine;
using UnityEngine.UI;
using UnityExplorer.Inspectors;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;

public class OperationsPage
{
    public GameObject UIRoot { get; set; }
    public FsmInspector Owner { get; set; }

    public GameObject ScrollView { get; set; }
    public GameObject ContentGroup { get; set; }
    OperationBlock ShowSelected { get; set; }
    OperationBlock Method { get; set; }
    OperationBlock Record { get; set; }

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

        page.ShowSelected = new OperationBlock(page, page.ContentGroup, OperationBlock.OperationBlockType.ShowSelected);
        page.Method = new OperationBlock(page, page.ContentGroup, OperationBlock.OperationBlockType.Method);
        page.Record = new OperationBlock(page, page.ContentGroup, OperationBlock.OperationBlockType.Record);








        return page;

    }
}
public class OperationBlock
{
    public enum OperationBlockType
    {
        ShowSelected,
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
        UIFactory.SetLayoutElement(NameRow, 200, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);

        SubContentButton = UIFactory.CreateButton(NameRow, "SubContentButton", "▲", subInactiveColor);
        UIFactory.SetLayoutElement(SubContentButton.Component.gameObject, minWidth: 25, minHeight: 25, flexibleWidth: 0, flexibleHeight: 0);
        SubContentButton.OnClick += SubContentClicked;


        Title = UIFactory.CreateLabel(NameRow, "ActionName", "notset", TextAnchor.MiddleLeft, Color.black);
        UIFactory.SetLayoutElement(Title.gameObject, minWidth: 150, flexibleWidth: 9999, flexibleHeight: 9999);
        Title.text = Type.ToString();

        // var inspect_button = UIFactory.CreateButton(name, "InspectButton", "Inspect");
        // UIFactory.SetLayoutElement(inspect_button.GameObject, minWidth: 80, flexibleWidth: 0, flexibleHeight: 9999);


        SubContentHolder = UIFactory.CreateVerticalGroup(UIRoot, "SubContentHolder", false, false, true, true);
        UIFactory.SetLayoutElement(SubContentHolder, minWidth: 300, flexibleWidth: 9999);
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


}