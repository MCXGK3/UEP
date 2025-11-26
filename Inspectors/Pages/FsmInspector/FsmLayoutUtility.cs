using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.UIElements;

public static class FsmLayoutUtility
{
    // 节点之间的最小间距（越大越松散）
    public const float MinSpacing = 50f;


    // 迭代次数（2~8 次都行）
    public const int MaxRelaxIterations = 50;

    public static void TestAddNewState(PlayMakerFSM fsm, string name, int num = 5)
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
    public static void AutoPlaceNode(FsmNode newNode, List<FsmNode> allNodes)
    {
        List<Vector2> poss = [newNode.RectTransform.anchoredPosition];
        // 多次迭代让节点逐步松散
        bool random = false;
        // for (int i = 0; i < RelaxIterations; i++)
        int cnt = 0;
        while (true)
        {
            if (!ApplyRelaxation(newNode, allNodes, random))
            {
                break;
            }
            cnt++;
            if (cnt > MaxRelaxIterations)
            {
                ("ERROR:Too many Relaxation for a FsmNode " + newNode.Target.Name).LogWarning();
                break;
            }

            random = false;
            Vector2 pos = newNode.RectTransform.anchoredPosition;
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
    private static bool ApplyRelaxation(FsmNode target, List<FsmNode> nodes, bool random = false)
    {
        RectTransform tr = target.RectTransform;
        Vector2 pos = tr.anchoredPosition;
        bool replaced = false;

        foreach (var other in nodes)
        {
            if (other == target)
                continue;

            if (!IsOverlapping(tr, other.RectTransform))
                continue;
            replaced = true;
            ("target:" + target.Target.Name + RectAABB(target.RectTransform)).LogInfo();
            ("other:" + other.Target.Name + RectAABB(other.RectTransform)).LogInfo();
            "".LogInfo();
            // 计算推力方向（从其他节点指向自己）
            Vector2 dir = (pos - (Vector2)other.RectTransform.anchoredPosition).normalized;
            bool same_pos = false;

            if (dir == Vector2.zero)
            {
                dir = new Vector2(0, -1).normalized;
                same_pos = true;
            }
            if (random)
            {
                dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            }

            float overlapAmount = GetOverlapDistance(tr, other.RectTransform);

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
