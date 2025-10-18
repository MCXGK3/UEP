using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer;
using UnityExplorer.Inspectors;
using UnityExplorer.Inspectors.MouseInspectors;
using UniverseLib.Utility;

namespace UnityExplorerPlus.Inspectors
{
	// Token: 0x02000056 RID: 86
	internal class EnemyInspector : MouseInspectorBase
	{
		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600016C RID: 364 RVA: 0x00009CC0 File Offset: 0x00007EC0
		public static Text objNameLabel
		{
			get
			{
				return Traverse.Create(MouseInspector.Instance).Field("objNameLabel").GetValue<Text>();
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x0600016D RID: 365 RVA: 0x00009CCD File Offset: 0x00007ECD
		public static Text objPathLabel
		{
			get
			{
				return Traverse.Create(MouseInspector.Instance).Field("objPathLabel").GetValue<Text>();
			}
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00009CDA File Offset: 0x00007EDA
		public override void OnBeginMouseInspect()
		{
			this.lastHit = null;
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00009CDA File Offset: 0x00007EDA
		public override void ClearHitData()
		{
			this.lastHit = null;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00009CE4 File Offset: 0x00007EE4
		public void UpdateText(GameObject go)
		{
			bool flag = go == null;
			if (flag)
			{
				Traverse.Create(MouseInspector.Instance).Method("ClearHitData").GetValue();
			}
			else
			{
				EnemyInspector.objNameLabel.text = "<b>Click to Inspect:</b> <color=cyan>" + go.name + "</color>";
				EnemyInspector.objPathLabel.text = "Path: " + UnityHelpers.GetTransformPath(go.transform, true);
			}
		}

		// Token: 0x06000171 RID: 369 RVA: 0x000039CC File Offset: 0x00001BCC
		public override void OnEndInspect()
		{
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00009D50 File Offset: 0x00007F50
		public override void UpdateMouseInspect(Vector2 _)
		{
			Camera cam = CameraSwitcher.GetCurrentCamera();
			bool flag = cam == null;
			if (flag)
			{
				MouseInspector.Instance.StopInspect();
			}
			else
			{
				Vector2 worldPos = CameraSwitcher.GetCurrentMousePosition();
				Collider2D hit = Enumerable.FirstOrDefault<Collider2D>(Enumerable.Where<Collider2D>(Physics2D.OverlapPointAll(worldPos, -1), (Collider2D x) => x.GetComponent<HealthManager>() != null));
				GameObject go = (hit != null) ? hit.gameObject : null;
				bool flag2 = go != this.lastHit;
				if (flag2)
				{
					this.lastHit = go;
					this.UpdateText(go);
				}
			}
		}

		// Token: 0x06000173 RID: 371 RVA: 0x00009DE8 File Offset: 0x00007FE8
		public override void OnSelectMouseInspect()
		{
			bool flag = this.lastHit != null;
			if (flag)
			{
				InspectorManager.Inspect(this.lastHit, null);
			}
		}

		// Token: 0x040000D1 RID: 209
		public GameObject lastHit = null;
	}
}
