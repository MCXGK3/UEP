using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.ObjectPool;
using UniverseLib.UI.Widgets.ScrollView;

public class TransitionCell : ICell
{
    public GameObject UIRoot { get; set; }

    public float DefaultHeight => 16f;
    public string Name => EventText.text;
    public Text EventText { get; set; }

    public bool Enabled => UIRoot.activeSelf;

    public RectTransform Rect { get; set; }
    public Color default_color = Color.black;

    public FsmTransition Target { get; set; }
    public bool InGlobalTransition { get; set; }
    public int color_index;
    ContentSizeFitter contentSizeFitter;

    public GameObject CreateContent(GameObject parent)
    {
        UIRoot = UIFactory.CreateHorizontalGroup(parent, "NodeEventCell", forceExpandWidth: true, forceExpandHeight: false, childControlWidth: true, childControlHeight: true, 2, default(Vector4), default_color, TextAnchor.MiddleCenter);
        Rect = UIRoot.GetComponent<RectTransform>();
        Rect.anchorMin = new Vector2(0.5f, 0.5f);
        Rect.anchorMax = new Vector2(0.5f, 0.5f);
        Rect.pivot = new Vector2(0.5f, 0.5f);
        Rect.sizeDelta = new Vector2(25f, 25f);
        contentSizeFitter = UIRoot.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        UIFactory.SetLayoutElement(UIRoot, 100, flexibleWidth: 9999, minHeight: 16, flexibleHeight: 0);
        EventText = UIFactory.CreateLabel(UIRoot, "NodeEventText", "NotSet", TextAnchor.MiddleCenter, Color.white);
        UIFactory.SetLayoutElement(EventText.gameObject, flexibleWidth: 9999, minHeight: 16, flexibleHeight: 0);
        return UIRoot;
    }
    public void OnReturnToPool()
    {
        UIRoot.GetComponent<Image>().color = Color.clear;
        Pool<TransitionCell>.Return(this);
        Rect.anchorMin = new Vector2(0.5f, 0.5f);
        Rect.anchorMax = new Vector2(0.5f, 0.5f);
        Rect.pivot = new Vector2(0.5f, 0.5f);
        EventText.color = Color.white;
    }

    public void Disable()
    {
        EventText.gameObject.SetActive(false);
    }

    public void Enable()
    {
        EventText.gameObject.SetActive(true);
    }

    public void SetTarget(FsmTransition transition, bool in_global_transition = false, bool has_no_dest = false, int color_index = 0)
    {
        Target = transition;
        EventText.text = transition.FsmEvent.Name;
        InGlobalTransition = in_global_transition;
        default_color = FsmLayoutUtility.GetColor(color_index);
        // if (color_index == 0) { default_color = Color.black; }
        this.color_index = color_index;
        UIRoot.GetComponent<Image>().color = in_global_transition ? Color.black : default_color;
        contentSizeFitter.enabled = false;
        if (has_no_dest)
        {
            UIRoot.GetComponent<Image>().color = new Color(1, 0, 1, 0.5f);
            EventText.color = Color.black;
        }
    }
}