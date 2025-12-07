using System;
using System.Reflection;
using UnityEngine;

namespace UnityExplorerPlus
{
	// Token: 0x0200000A RID: 10
	public static class Drawing
	{
		// Token: 0x06000016 RID: 22 RVA: 0x0000220C File Offset: 0x0000040C
		public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width, bool antiAlias)
		{
			float dx = pointB.x - pointA.x;
			float dy = pointB.y - pointA.y;
			float len = Mathf.Sqrt(dx * dx + dy * dy);
			bool flag = len < 0.001f;
			if (!flag)
			{
				Texture2D tex;
				Material mat;
				if (antiAlias)
				{
					width *= 3f;
					tex = Drawing.aaLineTex;
					mat = Drawing.blendMaterial;
				}
				else
				{
					tex = Drawing.lineTex;
					mat = Drawing.blitMaterial;
				}
				float wdx = width * dy / len;
				float wdy = width * dx / len;
				Matrix4x4 matrix = Matrix4x4.identity;
				matrix.m00 = dx;
				matrix.m01 = -wdx;
				matrix.m03 = pointA.x + 0.5f * wdx;
				matrix.m10 = dy;
				matrix.m11 = wdy;
				matrix.m13 = pointA.y - 0.5f * wdy;
				GL.PushMatrix();
				GL.MultMatrix(matrix);
				Graphics.DrawTexture(Drawing.lineRect, tex, Drawing.lineRect, 0, 0, 0, 0, color, mat);
				if (antiAlias)
				{
					Graphics.DrawTexture(Drawing.lineRect, tex, Drawing.lineRect, 0, 0, 0, 0, color, mat);
				}
				GL.PopMatrix();
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000233A File Offset: 0x0000053A
		public static void DrawCircle(Vector2 center, int radius, Color color, float width, int segmentsPerQuarter)
		{
			Drawing.DrawCircle(center, radius, color, width, false, segmentsPerQuarter);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000234C File Offset: 0x0000054C
		public static void DrawCircle(Vector2 center, int radius, Color color, float width, bool antiAlias, int segmentsPerQuarter)
		{
			float rh = (float)radius * 0.55191505f;
			Vector2 p;
			p = new(center.x, center.y - (float)radius);
			Vector2 p1_tan_a;
			p1_tan_a = new(center.x - rh, center.y - (float)radius);
			Vector2 p1_tan_b;
			p1_tan_b = new(center.x + rh, center.y - (float)radius);
			Vector2 p2;
			p2 = new(center.x + (float)radius, center.y);
			Vector2 p2_tan_a;
			p2_tan_a = new(center.x + (float)radius, center.y - rh);
			Vector2 p2_tan_b;
			p2_tan_b = new(center.x + (float)radius, center.y + rh);
			Vector2 p3;
			p3 = new(center.x, center.y + (float)radius);
			Vector2 p3_tan_a;
			p3_tan_a = new(center.x - rh, center.y + (float)radius);
			Vector2 p3_tan_b;
			p3_tan_b = new(center.x + rh, center.y + (float)radius);
			Vector2 p4;
			p4 = new(center.x - (float)radius, center.y);
			Vector2 p4_tan_a;
			p4_tan_a = new(center.x - (float)radius, center.y - rh);
			Vector2 p4_tan_b;
			p4_tan_b = new(center.x - (float)radius, center.y + rh);
			Drawing.DrawBezierLine(p, p1_tan_b, p2, p2_tan_a, color, width, antiAlias, segmentsPerQuarter);
			Drawing.DrawBezierLine(p2, p2_tan_b, p3, p3_tan_b, color, width, antiAlias, segmentsPerQuarter);
			Drawing.DrawBezierLine(p3, p3_tan_a, p4, p4_tan_b, color, width, antiAlias, segmentsPerQuarter);
			Drawing.DrawBezierLine(p4, p4_tan_a, p, p1_tan_a, color, width, antiAlias, segmentsPerQuarter);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000024C8 File Offset: 0x000006C8
		public static void DrawBezierLine(Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, bool antiAlias, int segments)
		{
			Vector2 lastV = Drawing.CubeBezier(start, startTangent, end, endTangent, 0f);
			for (int i = 1; i < segments + 1; i++)
			{
				Vector2 v = Drawing.CubeBezier(start, startTangent, end, endTangent, (float)i / (float)segments);
				Drawing.DrawLine(lastV, v, color, width, antiAlias);
				lastV = v;
			}
		}

		// Token: 0x0600001A RID: 26 RVA: 0x0000251C File Offset: 0x0000071C
		private static Vector2 CubeBezier(Vector2 s, Vector2 st, Vector2 e, Vector2 et, float t)
		{
			float rt = 1f - t;
			return rt * rt * rt * s + 3f * rt * rt * t * st + 3f * rt * t * t * et + t * t * t * e;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002584 File Offset: 0x00000784
		static Drawing()
		{
			Drawing.Initialize();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000025C4 File Offset: 0x000007C4
		private static void Initialize()
		{
			bool flag = Drawing.lineTex == null;
			if (flag)
			{
				Drawing.lineTex = new Texture2D(1, 1 /*,5, false*/);
				Drawing.lineTex.SetPixel(0, 1, Color.white);
				Drawing.lineTex.Apply();
			}
			bool flag2 = Drawing.aaLineTex == null;
			if (flag2)
			{
				Drawing.aaLineTex = new Texture2D(1, 3/*, 5, false*/);
				Drawing.aaLineTex.SetPixel(0, 0, new Color(1f, 1f, 1f, 0f));
				Drawing.aaLineTex.SetPixel(0, 1, Color.white);
				Drawing.aaLineTex.SetPixel(0, 2, new Color(1f, 1f, 1f, 0f));
				Drawing.aaLineTex.Apply();
			}
			Drawing.blitMaterial = (Material)typeof(GUI).GetMethod("get_blitMaterial", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
			Drawing.blendMaterial = (Material)typeof(GUI).GetMethod("get_blendMaterial", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
		}

		// Token: 0x04000007 RID: 7
		private static Texture2D aaLineTex = null;

		// Token: 0x04000008 RID: 8
		private static Texture2D lineTex = null;

		// Token: 0x04000009 RID: 9
		private static Material blitMaterial = null;

		// Token: 0x0400000A RID: 10
		private static Material blendMaterial = null;

		// Token: 0x0400000B RID: 11
		private static Rect lineRect = new Rect(0f, 0f, 1f, 1f);
	}
}
