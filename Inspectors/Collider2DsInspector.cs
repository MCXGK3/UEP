using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer.Inspectors;
using UnityExplorer.Inspectors.MouseInspectors;
using UniverseLib;

namespace UnityExplorerPlus.Inspectors
{
	// Token: 0x02000052 RID: 82
	internal class Collider2DsInspector : MouseInspectorBase
	{
		// Token: 0x0600015A RID: 346 RVA: 0x00009ACC File Offset: 0x00007CCC
		public override void OnBeginMouseInspect()
		{
			this.currentGameObjects.Clear();
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00009ADB File Offset: 0x00007CDB
		private IEnumerator SetPanelActiveCoro()
		{
			yield return null;
			this.resultPanel.SetActive(true);
			this.resultPanel.ShowResults();
			yield break;
		}

		// Token: 0x0600015C RID: 348 RVA: 0x00009ACC File Offset: 0x00007CCC
		public override void ClearHitData()
		{
			this.currentGameObjects.Clear();
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00009AEA File Offset: 0x00007CEA
		public override void OnSelectMouseInspect()
		{
			this.resultPanel.Result.Clear();
			this.resultPanel.Result.AddRange(this.currentGameObjects);
			RuntimeHelper.StartCoroutine(this.SetPanelActiveCoro());
		}

		// Token: 0x0600015E RID: 350 RVA: 0x00009B24 File Offset: 0x00007D24
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
				this.currentGameObjects.Clear();
				Vector2 p = CameraSwitcher.GetCurrentMousePosition();
				this.currentGameObjects.AddRange(Enumerable.Select<Collider2D, GameObject>(Physics2D.OverlapPointAll(p, -1), (Collider2D x) => x.gameObject));
				bool flag2 = this.currentGameObjects.Count > 0;
				if (flag2)
				{
					Traverse.Create(MouseInspector.Instance).Field("objNameLabel").GetValue<Text>().text = string.Format("Click to view collider2Ds under mouse{0}: {1}", p, this.currentGameObjects.Count);
				}
				else
				{
					Traverse.Create(MouseInspector.Instance).Field("objNameLabel").GetValue<Text>().text = "No collider2Ds under mouse.";
				}
			}
		}

		// Token: 0x0600015F RID: 351 RVA: 0x000039CC File Offset: 0x00001BCC
		public override void OnEndInspect()
		{
		}

		// Token: 0x040000CA RID: 202
		public Collider2DsInspectorResultPanel resultPanel = new Collider2DsInspectorResultPanel();

		// Token: 0x040000CB RID: 203
		private List<GameObject> currentGameObjects = new List<GameObject>();
	}
}
