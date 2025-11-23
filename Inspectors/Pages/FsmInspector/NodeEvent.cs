using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.ObjectPool;
using UniverseLib.UI.Widgets.ScrollView;

public class NodeEventCell : ICell
{
    public GameObject UIRoot { get; set; }

    public float DefaultHeight => 16f;
    public string Name => EventText.text;
    public Text EventText { get; set; }

    public bool Enabled => UIRoot.activeSelf;

    public RectTransform Rect { get; set; }

    public GameObject CreateContent(GameObject parent)
    {
        UIRoot = UIFactory.CreateHorizontalGroup(parent, "NodeEventCell", forceExpandWidth: true, forceExpandHeight: false, childControlWidth: true, childControlHeight: true, 2, default(Vector4), new Color(0.11f, 0.11f, 0.11f, 0.11f), TextAnchor.MiddleCenter);
        Rect = UIRoot.GetComponent<RectTransform>();
        Rect.anchorMin = new Vector2(0f, 1f);
        Rect.anchorMax = new Vector2(0f, 1f);
        Rect.pivot = new Vector2(0.5f, 1f);
        Rect.sizeDelta = new Vector2(25f, 25f);
        UIFactory.SetLayoutElement(UIRoot, 100, flexibleWidth: 9999, minHeight: 16, flexibleHeight: 0);
        EventText = UIFactory.CreateLabel(UIRoot, "NodeEventText", "NotSet", TextAnchor.MiddleCenter, Color.white);
        UIFactory.SetLayoutElement(EventText.gameObject, flexibleWidth: 9999, minHeight: 16, flexibleHeight: 0);
        return UIRoot;
    }

    public void Disable()
    {
        EventText.gameObject.SetActive(false);
    }

    public void Enable()
    {
        EventText.gameObject.SetActive(true);
    }

    public void SetTarget(string name)
    {
        EventText.text = name;
    }
}