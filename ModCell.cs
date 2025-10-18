

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer;
using UnityExplorer.CacheObject;
using UniverseLib.UI.Models;
using UniverseLib.UI.ObjectPool;
using UniverseLib.UI.Widgets.ScrollView;

internal class ModCell : ICell, IPooledObject
{
    public ButtonRef nameButton;
    public bool Enabled { get; private set; }

    public (MonoBehaviour, Type) bindMod;

    public Text nameText;

    public RectTransform Rect { get; set; }
    public GameObject UIRoot { get; set; }

    public float DefaultHeight => 25f;

    public GameObject CreateContent(GameObject parent)
    {
        throw new System.NotImplementedException();
    }
    public void OnClick()
    {
        InspectorManager.Inspect(((object)bindMod.Item1 ?? ((object)bindMod.Item2)), (CacheObjectBase)null);
    }
    public void BindMod((MonoBehaviour, Type) mod)
    {

        bindMod = mod;
        if (mod.Item1 != null)
        {
            nameButton.ButtonText.text = "<i><color=green>" + mod.Item1.GetName() + "</color><color=grey>(" + mod.Item2.FullName + ")</color></i>";
        }
        else
        {
            nameButton.ButtonText.text = "<i><color=red>Failed to load</color><color=grey>(" + mod.Item2.FullName + ")</color></i>";
        }
        ((Graphic)nameButton.ButtonText).color = Color.white;
    }
    public void Disable()
    {
        Enabled = false;
        UIRoot.SetActive(false);
    }

    public void Enable()
    {
        Enabled = true;
        UIRoot.SetActive(true);
    }
}