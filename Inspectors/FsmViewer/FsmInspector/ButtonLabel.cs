using System;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.ObjectPool;

public class ButtonLabel : IPooledObject
{
    public GameObject UIRoot { get; set; }
    public RectTransform Rect { get; set; }

    public float DefaultHeight => 25f;
    public Text Label { get; set; }
    public ButtonRef Button { get; set; }

    public GameObject CreateContent(GameObject parent)
    {
        UIRoot = UIFactory.CreateHorizontalGroup(parent, "ButtonLabel", false, false, true, true);
        Rect = UIRoot.transform as RectTransform;
        UIFactory.SetLayoutElement(UIRoot, flexibleWidth: 9999, minHeight: 40);

        Label = UIFactory.CreateLabel(UIRoot, "Label", "NotSet", TextAnchor.MiddleLeft);
        UIFactory.SetLayoutElement(Label.gameObject, flexibleWidth: 9999, flexibleHeight: 9999);

        Button = UIFactory.CreateButton(UIRoot, "Button", "NotSet");
        UIFactory.SetLayoutElement(Button.GameObject, minWidth: 80, flexibleWidth: 0, minHeight: 25, flexibleHeight: 9999);


        return UIRoot;
    }
    public static ButtonLabel OnBorredFromPool(GameObject parent, string text, Action button_action = null, string button_text = null)
    {
        var bl = Pool<ButtonLabel>.Borrow();
        bl.Rect.SetParent(parent.transform, false);
        if (button_action == null)
        {
            bl.Button.GameObject.SetActive(false);
        }
        else
        {
            bl.Button.GameObject.SetActive(true);
            bl.Button.OnClick += button_action;
            bl.Button.ButtonText.text = button_text;
        }
        bl.Label.text = text;
        return bl;
    }
    public void OnReturnToPool()
    {
        Button.OnClick = null;
        Label.color = Color.white;
        Pool<ButtonLabel>.Return(this);
    }
}