using System;
using UEP;
using UnityEngine;
using UnityEngine.EventSystems;

public class CheckScroll : MonoBehaviour, IScrollHandler
{
    public Action<Vector2> returnScrollData = null;
    //法三：
    public void OnScroll(PointerEventData eventData)
    {
        if (returnScrollData != null)
        {
            returnScrollData(eventData.scrollDelta);
        }
    }
}