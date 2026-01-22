using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UEP;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer.Inspectors;
using UnityExplorer.Inspectors.MouseInspectors;
using UnityExplorerPlus.LineDrawing;
using UniverseLib;

namespace UnityExplorerPlus.Inspectors
{
	// Token: 0x02000059 RID: 89
	internal class RendererInspector : MouseInspectorBase, ILineProvider
	{
		// Token: 0x0600018B RID: 395 RVA: 0x0000A16C File Offset: 0x0000836C
		public RendererInspector()
		{
			UEPPlugin.Instance.gameObject.AddComponent<LineRenderer2>();
		}
		public static Text objNameLabel
		{
			get
			{
				return Traverse.Create(MouseInspector.Instance).Field("objNameLabel").GetValue<Text>();
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600018C RID: 396 RVA: 0x0000A1C5 File Offset: 0x000083C5
		public List<UnityExplorerPlus.LineDrawing.LineData> Lines { get; } = new List<UnityExplorerPlus.LineDrawing.LineData>();

		// Token: 0x0600018D RID: 397 RVA: 0x0000A1CD File Offset: 0x000083CD
		public override void OnEndInspect()
		{
			this.Lines.Clear();
			this.cacheTime = 0f;
			this.rendererCache = null;
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000A1EE File Offset: 0x000083EE
		public override void OnBeginMouseInspect()
		{
			this.currentGameObjects.Clear();
			this.cacheTime = 0f;
		}

		// Token: 0x0600018F RID: 399 RVA: 0x0000A208 File Offset: 0x00008408
		private IEnumerator SetPanelActiveCoro()
		{
			yield return null;
			this.resultPanel.SetActive(true);
			this.resultPanel.ShowResults();
			yield break;
		}

		// Token: 0x06000190 RID: 400 RVA: 0x0000A217 File Offset: 0x00008417
		public override void ClearHitData()
		{
			this.currentGameObjects.Clear();
		}

		// Token: 0x06000191 RID: 401 RVA: 0x0000A226 File Offset: 0x00008426
		public override void OnSelectMouseInspect(Action<GameObject> inspectorAction)
		{
			this.resultPanel.Result.Clear();
			this.resultPanel.Result.AddRange(this.currentGameObjects);
			RuntimeHelper.StartCoroutine(this.SetPanelActiveCoro());
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000A260 File Offset: 0x00008460
		public static bool TestPointInTrig(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
		{
			float signOfTrig = (b.x - a.x) * (c.y - a.y) - (b.y - a.y) * (c.x - a.x);
			float signOfAB = (b.x - a.x) * (p.y - a.y) - (b.y - a.y) * (p.x - a.x);
			float signOfCA = (a.x - c.x) * (p.y - c.y) - (a.y - c.y) * (p.x - c.x);
			float signOfBC = (c.x - b.x) * (p.y - c.y) - (c.y - b.y) * (p.x - c.x);
			bool d = signOfAB * signOfTrig > 0f;
			bool d2 = signOfCA * signOfTrig > 0f;
			bool d3 = signOfBC * signOfTrig > 0f;
			return d && d2 && d3;
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000A380 File Offset: 0x00008580
		private Vector2 LocalToScreenPoint(Vector3 point)
		{
			Vector2 result = CameraSwitcher.GetCurrentCamera().WorldToScreenPoint(point);
			return new Vector2((float)((int)Math.Round((double)result.x)), (float)((int)Math.Round((double)((float)Screen.height - result.y))));
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000A3CC File Offset: 0x000085CC
		private void AddLine(Vector2 a, Vector2 b, float z)
		{
			bool flag = !false;
			if (!flag)
			{
				float c = (float)Mathf.RoundToInt(z * 50f % 255f) / 255f;
				Color color = new(1, c, c, 1);
				this.Lines.Add(new UnityExplorerPlus.LineDrawing.LineData(this.LocalToScreenPoint(a), this.LocalToScreenPoint(b), color, Mathf.RoundToInt(z * 100f), 0.7f));
			}
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000A460 File Offset: 0x00008660
		public override void UpdateMouseInspect(Vector2 _)
		{
			bool flag = Time.unscaledTime - this.cacheTime > 0.5f || this.rendererCache == null;
			if (flag)
			{
				this.rendererCache = UnityEngine.Object.FindObjectsOfType<Renderer>();
			}
			Camera cam = CameraSwitcher.GetCurrentCamera();
			bool flag2 = cam == null;
			if (flag2)
			{
				MouseInspector.Instance.StopInspect();
			}
			else
			{
				this.currentGameObjects.Clear();
				this.Lines.Clear();
				Vector2 p = CameraSwitcher.GetCurrentMousePosition();
				foreach (Renderer v in Enumerable.OrderBy<Renderer, float>(Enumerable.Where<Renderer>(Enumerable.Where<Renderer>(Enumerable.Where<Renderer>(Enumerable.Where<Renderer>(this.rendererCache, (Renderer x) => x != null), (Renderer x) => x.isVisible), (Renderer x) => x.enabled), (Renderer x) => x.gameObject.activeInHierarchy), (Renderer x) => x.transform.position.z))
				{
					Vector2 pos = v.transform.position;
					Vector3 pos2 = v.transform.position;
					Vector3 scale = new(v.transform.GetScaleX(), v.transform.GetScaleY(), 1);
					List<ValueTuple<Vector3, Vector3, Vector3>> vert = new List<ValueTuple<Vector3, Vector3, Vector3>>();
					MeshRenderer mr = v as MeshRenderer;
					bool flag3 = mr != null;
					if (flag3)
					{
						MeshFilter filter = v.GetComponent<MeshFilter>();
						bool flag4 = filter == null;
						if (!flag4)
						{
							Mesh mesh = filter.sharedMesh;
							bool flag5 = mesh == null;
							if (!flag5)
							{
								Vector3[] points = mesh.vertices;
								vert.Clear();
								bool isTouch = false;
								for (int i = 0; i < mesh.subMeshCount; i++)
								{
									int[] trig = filter.sharedMesh.GetTriangles(i);
									for (int i2 = 0; i2 < trig.Length; i2 += 3)
									{
										Vector3 a = Extensions.MultiplyElements(points[trig[i]], scale) + (Vector3)pos;
										Vector3 b = Extensions.MultiplyElements(points[trig[i + 1]], scale) + (Vector3)pos;
										Vector3 c = Extensions.MultiplyElements(points[trig[i + 2]], scale) + (Vector3)pos;
										vert.Add(new ValueTuple<Vector3, Vector3, Vector3>(a, b, c));
										bool flag6 = !isTouch && RendererInspector.TestPointInTrig(a, b, c, p);
										if (flag6)
										{
											this.currentGameObjects.Add(v.gameObject);
											isTouch = true;
										}
									}
								}
								bool flag7 = isTouch;
								if (flag7)
								{
									foreach (ValueTuple<Vector3, Vector3, Vector3> valueTuple in vert)
									{
										Vector3 a2 = valueTuple.Item1;
										Vector3 b2 = valueTuple.Item2;
										Vector3 c2 = valueTuple.Item3;
										this.AddLine(a2, b2, pos2.z);
										this.AddLine(b2, c2, pos2.z);
										this.AddLine(a2, c2, pos2.z);
									}
								}
							}
						}
					}
					else
					{
						SpriteRenderer sprite = v as SpriteRenderer;
						bool flag8 = sprite != null;
						if (flag8)
						{
							bool flag9 = sprite.sprite == null;
							if (!flag9)
							{
								Vector2[] points2 = sprite.sprite.vertices;
								ushort[] trig2 = sprite.sprite.triangles;
								vert.Clear();
								bool isTouch2 = false;
								for (int j = 0; j < trig2.Length; j += 3)
								{
									Vector2 a3 = Extensions.MultiplyElements(points2[(int)trig2[j]], (Vector2)scale) + pos;
									Vector2 b3 = Extensions.MultiplyElements(points2[(int)trig2[j + 1]], (Vector2)scale) + pos;
									Vector2 c3 = Extensions.MultiplyElements(points2[(int)trig2[j + 2]], (Vector2)scale) + pos;
									vert.Add(new ValueTuple<Vector3, Vector3, Vector3>(a3, b3, c3));
									bool flag10 = !isTouch2 && RendererInspector.TestPointInTrig(a3, b3, c3, p);
									if (flag10)
									{
										this.currentGameObjects.Add(v.gameObject);
										isTouch2 = true;
									}
								}
								bool flag11 = isTouch2;
								if (flag11)
								{
									foreach (ValueTuple<Vector3, Vector3, Vector3> valueTuple2 in vert)
									{
										Vector3 a4 = valueTuple2.Item1;
										Vector3 b4 = valueTuple2.Item2;
										Vector3 c4 = valueTuple2.Item3;
										this.AddLine(a4, b4, pos2.z);
										this.AddLine(b4, c4, pos2.z);
										this.AddLine(a4, c4, pos2.z);
									}
								}
							}
						}
					}
				}
				bool flag12 = this.currentGameObjects.Count > 0;
				if (flag12)
				{

					objNameLabel.text = string.Format("Click to view renderers under mouse{0}: {1}", p, this.currentGameObjects.Count);
				}
				else
				{
					objNameLabel.text = "No renderers under mouse.";
				}
			}
		}

		// Token: 0x040000D7 RID: 215
		public RendererInspectorResultPanel resultPanel = new RendererInspectorResultPanel();

		// Token: 0x040000D8 RID: 216
		private List<GameObject> currentGameObjects = new List<GameObject>();

		// Token: 0x040000D9 RID: 217
		public Renderer[] rendererCache = null;

		// Token: 0x040000DA RID: 218
		public float cacheTime = 0f;
	}
}
