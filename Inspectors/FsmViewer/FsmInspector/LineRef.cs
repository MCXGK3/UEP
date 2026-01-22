using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityExplorer;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.ObjectPool;
public class GraphicLine : MaskableGraphic
{
    public enum CurveType
    {
        Line,
        QuadraticBezier,
        CubicBezier
    }

    [Header("Curve Type")]
    public CurveType curveType = CurveType.Line;

    [Header("Control Points (local Vector2)")]
    private Vector2 p0;

    private Vector2 p1;
    private Vector2 p2;
    private Vector2 p3;

    public Vector2 P0
    {
        get => p0;
        set
        {
            if (p0 != value)
            {
                should_autoresize = true;
                p0 = value;
            }

        }
    }
    public Vector2 P1
    {
        get => p1;
        set
        {
            if (p1 != value)
            {
                should_autoresize = true;
                p1 = value;
            }

        }
    }
    public Vector2 P2
    {
        get => p2;
        set
        {
            if (p2 != value)
            {
                should_autoresize = true;
                p2 = value;
            }

        }
    }
    public Vector2 P3
    {
        get => p3;
        set
        {
            if (p3 != value)
            {
                should_autoresize = true;
                p3 = value;
            }

        }
    }

    [Header("Curve Settings")]
    public int segments = 50;
    public float lineWidth = 2f;

    [Header("Arrow Settings")]
    public bool showArrow = true;
    public float arrowLength = 10f;
    public float arrowWidth = 7.5f;

    private bool resize_scheduled = false;
    private bool should_autoresize = false;
    private List<Vector2> last_points = new();

    public override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        List<Vector2> points = new List<Vector2>();

        switch (curveType)
        {
            case CurveType.Line:
                points.Add(P0);
                points.Add(P1);
                break;

            case CurveType.QuadraticBezier:
                GenerateQuadraticBezier(points);
                break;

            case CurveType.CubicBezier:
                GenerateCubicBezier(points);
                break;
        }
        if (!resize_scheduled && should_autoresize)
        {
            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
            last_points = new List<Vector2>(points);
            resize_scheduled = true;
        }

        // AutoResizeRectTransform(points);

        //--- draw line segments ---//
        for (int i = 0; i < points.Count - 1; i++)
        {
            DrawLine(vh, points[i], points[i + 1], lineWidth);
        }

        //--- draw arrow ---//
        if (showArrow && points.Count >= 2)
        {
            DrawArrow(vh, points[points.Count - 2], points[points.Count - 1]);
        }
    }

    //==============================
    // Bezier Generators
    //==============================

    void GenerateQuadraticBezier(List<Vector2> result)
    {
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector2 pos =
                Mathf.Pow(1 - t, 2) * P0 +
                2 * (1 - t) * t * P1 +
                t * t * P2;

            result.Add(pos);
        }
    }

    void GenerateCubicBezier(List<Vector2> result)
    {
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector2 pos =
                Mathf.Pow(1 - t, 3) * P0 +
                3 * Mathf.Pow(1 - t, 2) * t * P1 +
                3 * (1 - t) * t * t * P2 +
                t * t * t * P3;

            result.Add(pos);
        }
    }

    //==============================
    // Mesh drawing functions
    //==============================

    void DrawLine(VertexHelper vh, Vector2 start, Vector2 end, float width)
    {
        Vector2 dir = (end - start).normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x) * width * 0.5f;

        Vector2 v0 = start + normal;
        Vector2 v1 = start - normal;
        Vector2 v2 = end + normal;
        Vector2 v3 = end - normal;

        int idx = vh.currentVertCount;

        vh.AddVert(v0, color, Vector2.zero);
        vh.AddVert(v1, color, Vector2.zero);
        vh.AddVert(v2, color, Vector2.zero);
        vh.AddVert(v3, color, Vector2.zero);

        vh.AddTriangle(idx + 0, idx + 1, idx + 2);
        vh.AddTriangle(idx + 2, idx + 1, idx + 3);
    }
    private float margin = 10f; // 额外边距，防止太贴边

    private void AutoResizeRectTransform(List<Vector2> points)
    {
        if (points == null || points.Count == 0)
            return;

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        // 找包围盒
        foreach (var p in points)
        {
            if (p.x < minX) minX = p.x;
            if (p.x > maxX) maxX = p.x;
            if (p.y < minY) minY = p.y;
            if (p.y > maxY) maxY = p.y;
        }

        // 扩展区域
        minX -= margin;
        minY -= margin;
        maxX += margin;
        maxY += margin;

        float width = maxX - minX;
        float height = maxY - minY;

        RectTransform rt = rectTransform;

        // 更新sizeDelta
        rt.sizeDelta = new Vector2(width, height);

        // 重设pivot 保持点对应
        rt.pivot = new Vector2(
            -minX / width,
            -minY / height
        );
        should_autoresize = false;
    }

    bool IsPointNearLine(Vector2 point, List<Vector2> line, float threshold)
    {
        for (int i = 0; i < line.Count - 1; i++)
        {
            Vector2 a = line[i];
            Vector2 b = line[i + 1];

            float dist = DistancePointToSegment(point, a, b);
            if (dist <= threshold) return true;
        }
        return false;
    }
    float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ap = p - a;
        Vector2 ab = b - a;
        float t = Mathf.Clamp01(Vector2.Dot(ap, ab) / ab.sqrMagnitude);
        return Vector2.Distance(p, a + ab * t);
    }
    public override bool Raycast(Vector2 sp, Camera eventCamera)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
        rectTransform, sp, eventCamera, out Vector2 local);
        return IsPointNearLine(local, last_points, lineWidth * 1.2f);
    }
    void DrawArrow(VertexHelper vh, Vector2 from, Vector2 to)
    {
        Vector2 dir = (to - from).normalized;

        Vector2 tip = to;
        Vector2 baseCenter = tip - dir * arrowLength;

        Vector2 normal = new Vector2(-dir.y, dir.x);

        Vector2 baseLeft = baseCenter + normal * (arrowWidth * 0.5f);
        Vector2 baseRight = baseCenter - normal * (arrowWidth * 0.5f);

        int idx = vh.currentVertCount;

        vh.AddVert(tip, color, Vector2.zero);
        vh.AddVert(baseLeft, color, Vector2.zero);
        vh.AddVert(baseRight, color, Vector2.zero);

        vh.AddTriangle(idx + 0, idx + 1, idx + 2);
    }

    public override void Rebuild(CanvasUpdate update)
    {
        base.Rebuild(update);
        if (update == CanvasUpdate.PostLayout)
        {
            if (should_autoresize)
                AutoResizeRectTransform(last_points);

            resize_scheduled = false;
        }
    }
    public void Refresh()
    {
        SetVerticesDirty();
    }
}


public class SelectableLine : Selectable, IPointerClickHandler
{
    public Action OnLineClick;
    public Action OnLineSelect;
    public Action OnLineDeselect;
    public void OnPointerClick(PointerEventData eventData)
    {
        OnLineClick?.Invoke();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        OnLineSelect?.Invoke();
    }
    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        OnLineDeselect?.Invoke();
    }

    public void ClearAction()
    {
        OnLineClick = null;
        OnLineSelect = null;
        OnLineDeselect = null;
    }
}

public class LineRef : IPooledObject
{
    //
    // 摘要:
    //     The actual Button component this object is a reference to.
    public SelectableLine SLine { get; set; }

    public GraphicLine GLine { get; set; }

    //
    // 摘要:
    //     The RectTransform for this Button.
    public RectTransform Rect => SLine.transform.TryCast<RectTransform>();
    public static ColorBlock default_colors = new()
    {
        normalColor = new Color(0.2f, 0.2f, 0.2f),
        selectedColor = new Color(0.2f, 0.2f, 0.2f),
        pressedColor = Color.white,
        highlightedColor = Color.white,
        disabledColor = Color.white,
        colorMultiplier = 1f
    };
    public Color CurrentNormalColor { get; set; } = default_colors.normalColor;



    public bool Enabled => UIRoot.activeSelf;

    public void Select()
    {
        SLine.interactable = false;

    }
    public void UnSelect()
    {
        SLine.interactable = true;
    }
    public void Mark()
    {
        RuntimeHelper.SetColorBlock(SLine, Color.cyan);
    }
    public void UnMark()
    {
        RuntimeHelper.SetColorBlock(SLine, CurrentNormalColor);
    }




    public GameObject UIRoot { get; set; }

    public float DefaultHeight => 10f;

    internal static void SetDefaultSelectableValues(Selectable selectable)
    {
        Navigation navigation = selectable.navigation;
        navigation.mode = Navigation.Mode.Explicit;
        selectable.navigation = navigation;
        RuntimeHelper.SetColorBlock(selectable, default_colors);
    }
    public void SetPath(List<Vector2> path, bool should_arrow = true, Color? normal_color = null)
    {
        int len = path.Count;
        if (len < 2 || len > 4) return;
        switch (len)
        {
            case 2:
                GLine.curveType = GraphicLine.CurveType.Line;
                GLine.P0 = path[0];
                GLine.P1 = path[1];
                break;
            case 3:
                GLine.curveType = GraphicLine.CurveType.QuadraticBezier;
                GLine.P0 = path[0];
                GLine.P1 = path[1];
                GLine.P2 = path[2];
                break;
            case 4:
                GLine.curveType = GraphicLine.CurveType.CubicBezier;
                GLine.P0 = path[0];
                GLine.P1 = path[1];
                GLine.P2 = path[2];
                GLine.P3 = path[3];
                break;
            default:
                break;
        }
        if (normal_color.HasValue)
        {
            CurrentNormalColor = normal_color.Value;
            RuntimeHelper.SetColorBlock(SLine, CurrentNormalColor);
            // GLine.color = CurrentNormalColor;
        }
        GLine.Refresh();
    }
    public static LineRef OnBorrowedFromPool(GameObject parent)
    {
        var line_ref = Pool<LineRef>.Borrow();
        line_ref.Rect.SetParent(parent.GetComponent<RectTransform>(), false);
        line_ref.Rect.SetAsFirstSibling();
        return line_ref;

    }
    public void OnReturnToPool()
    {
        SetPath([Vector2.zero, Vector2.zero], false, default_colors.normalColor);
        RuntimeHelper.SetColorBlock(SLine, default_colors);
        SLine.ClearAction();
        UnSelect();
        UnMark();
        Pool<LineRef>.Return(this);
    }

    public GameObject CreateContent(GameObject parent)
    {
        UIRoot = UIFactory.CreateUIObject("Line", parent);
        GLine = UIRoot.AddComponent<GraphicLine>();
        SLine = UIRoot.AddComponent<SelectableLine>();
        SetDefaultSelectableValues(SLine);
        SetPath([Vector2.zero, Vector2.zero]);
        return UIRoot;

    }
}
