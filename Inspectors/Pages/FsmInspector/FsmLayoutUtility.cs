using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using UnityEngine.UIElements;

public static class FsmLayoutUtility
{
    // 节点之间的最小间距（越大越松散）
    public const float MinSpacing = 50f;


    // 迭代次数（2~8 次都行）
    public const int MaxRelaxIterations = 50;

    public static readonly List<Color> STATE_COLORS = [
        new Color(0.1f,0.1f,0.1f),
        //  // 深蓝灰
        // new Color(0.2549f, 0.3725f, 0.4706f),
    
        // // 墨绿
        // new Color(0.2353f, 0.3922f, 0.3922f),
    
        // // 灰褐
        // new Color(0.3922f, 0.3529f, 0.3333f),
    
        // // 深紫灰
        // new Color(0.3137f, 0.2549f, 0.3922f),
    
        // // 炭灰
        // new Color(0.2941f, 0.2941f, 0.2941f),
    
        // // 深红褐
        // new Color(0.4510f, 0.2549f, 0.2941f),
    
        // // 深青灰
        // new Color(0.2157f, 0.3922f, 0.4314f),
    
        // // 橄榄褐
        // new Color(0.3529f, 0.3529f, 0.2549f),
        // 深靛蓝
    new Color(0.2745f, 0.3137f, 0.4706f),
    
    // 赭石棕
    new Color(0.4314f, 0.3529f, 0.2549f),
    
    // 暗酒红
    new Color(0.3922f, 0.2353f, 0.3137f),
    
    // 深青绿
    new Color(0.2157f, 0.3922f, 0.3529f),
    
    // 暗钴蓝
    new Color(0.2549f, 0.3137f, 0.5098f),
    
    // 深铜色
    new Color(0.4706f, 0.3137f, 0.2353f),
    
    // 梅子紫
    new Color(0.3922f, 0.2353f, 0.3922f),
    
    // 深橄榄绿
    new Color(0.3137f, 0.3529f, 0.2353f),
    
    // 灰蓝色
    new Color(0.3333f, 0.4118f, 0.5098f),
    
    // 砖红色
    new Color(0.4706f, 0.2745f, 0.2353f),
    
    // 深蓝绿
    new Color(0.2353f, 0.3922f, 0.4314f),
    
    // 深紫红
    new Color(0.4314f, 0.2353f, 0.3922f)
    ];
    public static readonly List<Color> TRANSITION_COLORS = [

    ];
    public static Color GetColor(int color_index)
    {
        if (color_index >= STATE_COLORS.Count)
        {
            ("Need Add More Color, At Least " + color_index).LogWarning();
            return STATE_COLORS.Last();
        }
        return STATE_COLORS[color_index];
    }

    public static void TestAddNewState(PlayMakerFSM fsm, string name, int num = 1)
    {
        List<FsmState> states = fsm.FsmStates.ToList();
        FsmState last_state = null;
        for (int i = 0; i < num; i++)
        {
            var tname = name;
            for (int j = 0; j < i; j++)
            {
                tname += j;
            }

            var t = new FsmState(fsm.Fsm);
            t.Name = tname;
            if (last_state != null)
            {
                List<FsmTransition> transitions = last_state.Transitions.ToList();
                for (int j = 0; j < i; j++)
                {
                    var trans = new FsmTransition();
                    trans.FsmEvent = FsmEvent.Finished;
                    if (j == 0)
                    {
                        trans.toState = tname;
                        trans.ToFsmState = t;
                    }
                    transitions.Add(trans);
                }
                last_state.Transitions = transitions.ToArray();
            }
            states.Add(t);
            last_state = t;
        }
        fsm.Fsm.States = states.ToArray();
    }

    /// <summary>
    /// 生成新节点后调用：自动避免重叠并使其排列松散。
    /// </summary>
    public static void AutoPlaceNode(RectTransform newrect, string name, List<RectTransform> rects, System.Random rng)
    {
        List<Vector2> poss = [newrect.anchoredPosition];
        // 多次迭代让节点逐步松散
        bool randomed = false;
        bool random = false;
        // for (int i = 0; i < RelaxIterations; i++)
        int cnt = 0;
        while (true)
        {
            if (!ApplyRelaxation(newrect, rects, random, rng))
            {
                break;
            }
            cnt++;
            random = false;
            Vector2 pos = newrect.anchoredPosition;
            if (cnt > MaxRelaxIterations)
            {
                if (!randomed)
                {
                    random = true;
                    randomed = true;
                }
                else
                {
                    ("ERROR:Too many Relaxation for " + name).LogWarning();
                    break;
                }
            }

            if (poss.Contains(pos))
            {
                // i = 0;
                cnt = 0;
                random = true;
                poss.Clear();
            }
            poss.Add(pos);
        }
    }

    /// <summary>
    /// 单次松散迭代：对每个重叠的节点施加推力。
    /// </summary>
    private static bool ApplyRelaxation(RectTransform target, List<RectTransform> rects, bool random = false, System.Random rng = null)
    {
        RectTransform tr = target;
        Vector2 pos = tr.anchoredPosition;
        bool replaced = false;

        foreach (var other in rects)
        {
            if (other == target)
                continue;

            if (!IsOverlapping(tr, other))
                continue;
            replaced = true;
            // ("target:" + target.Target.Name + RectAABB(target.RectTransform)).LogInfo();
            // ("other:" + other.Target.Name + RectAABB(other.RectTransform)).LogInfo();
            // "".LogInfo();
            // 计算推力方向（从其他节点指向自己）
            Vector2 dir = (pos - (Vector2)other.anchoredPosition).normalized;
            bool same_pos = false;

            if (dir == Vector2.zero)
            {
                dir = new Vector2(0, -1).normalized;
                same_pos = true;
            }
            if (random)
            {
                dir = new Vector2((float)(rng.NextDouble() * 2 - 1), (float)(rng.NextDouble() * 2 - 1)).normalized;
            }

            float overlapAmount = GetOverlapDistance(tr, other);

            // 推力：重叠越深，推得越强
            float push = (same_pos ? MinSpacing : overlapAmount) + MinSpacing;

            pos += dir * push;
        }

        tr.anchoredPosition = pos;
        return replaced;
    }

    /// <summary>
    /// 判断两个 RectTransform 是否重叠。
    /// </summary>
    private static bool IsOverlapping(RectTransform a, RectTransform b)
    {
        return RectAABB(a).Overlaps(RectAABB(b));
    }

    /// <summary>
    /// 根据 RectTransform 计算真实矩形坐标（UI 空间）
    /// </summary>
    private static Rect RectAABB(RectTransform rt)
    {
        Rect rect = rt.rect;
        rect.center += rt.anchoredPosition;
        return rect;
    }

    /// <summary>
    /// 获取两个矩形重叠的深度（用于计算推力大小）
    /// </summary>
    private static float GetOverlapDistance(RectTransform a, RectTransform b)
    {
        Rect ra = RectAABB(a);
        Rect rb = RectAABB(b);

        float xOverlap = Mathf.Min(ra.xMax, rb.xMax) - Mathf.Max(ra.xMin, rb.xMin);
        float yOverlap = Mathf.Min(ra.yMax, rb.yMax) - Mathf.Max(ra.yMin, rb.yMin);

        return Mathf.Min(xOverlap, yOverlap);
    }
}
