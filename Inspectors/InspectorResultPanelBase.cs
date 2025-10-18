using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityExplorer;
using UnityExplorer.UI;
using UnityExplorer.UI.Panels;
using UniverseLib.UI;
using UniverseLib.UI.Widgets.ButtonList;
using UniverseLib.UI.Widgets.ScrollView;
using UniverseLib.Utility;

namespace UnityExplorerPlus.Inspectors
{
	// Token: 0x02000058 RID: 88
	internal class InspectorResultPanelBase : UEPanel
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000178 RID: 376 RVA: 0x00009E40 File Offset: 0x00008040
		public override UEUIManager.Panels PanelType
		{
			get
			{
				return UEUIManager.Panels.UIInspectorResults;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000179 RID: 377 RVA: 0x00009E53 File Offset: 0x00008053
		public virtual List<GameObject> Result { get; } = new List<GameObject>();

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600017A RID: 378 RVA: 0x00009E5C File Offset: 0x0000805C
		public override string Name
		{
			get
			{
				return "UI Inspector Results";
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600017B RID: 379 RVA: 0x00009E74 File Offset: 0x00008074
		public override int MinWidth
		{
			get
			{
				return 500;
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600017C RID: 380 RVA: 0x00009E8C File Offset: 0x0000808C
		public override int MinHeight
		{
			get
			{
				return 500;
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600017D RID: 381 RVA: 0x00009EA4 File Offset: 0x000080A4
		public override Vector2 DefaultAnchorMin
		{
			get
			{
				return new Vector2(0.5f, 0.5f);
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600017E RID: 382 RVA: 0x00009EC8 File Offset: 0x000080C8
		public override Vector2 DefaultAnchorMax
		{
			get
			{
				return new Vector2(0.5f, 0.5f);
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600017F RID: 383 RVA: 0x00009EEC File Offset: 0x000080EC
		public override bool CanDragAndResize
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000180 RID: 384 RVA: 0x00009F00 File Offset: 0x00008100
		public override bool NavButtonWanted
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000181 RID: 385 RVA: 0x00009F14 File Offset: 0x00008114
		public override bool ShouldSaveActiveState
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000182 RID: 386 RVA: 0x00009F28 File Offset: 0x00008128
		public override bool ShowByDefault
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00009F3B File Offset: 0x0000813B
		public InspectorResultPanelBase() : base(Traverse.CreateWithType("UnityExplorer.UI.UIManager").Property("UiBase").GetValue<UIBase>())
		{


		}

		// Token: 0x06000184 RID: 388 RVA: 0x00009F55 File Offset: 0x00008155
		public void ShowResults()
		{
			this.dataHandler.RefreshData();
			this.buttonScrollPool.Refresh(true, true);
		}

		// Token: 0x06000185 RID: 389 RVA: 0x00009F74 File Offset: 0x00008174
		private List<GameObject> GetEntries()
		{
			return this.Result;
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00009F8C File Offset: 0x0000818C
		private bool ShouldDisplayCell(object cell, string filter)
		{
			return true;
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00009FA0 File Offset: 0x000081A0
		private void OnCellClicked(int index)
		{
			bool flag = index >= this.Result.Count;
			bool flag2 = !flag;
			if (flag2)
			{
				InspectorManager.Inspect(this.Result[index], null);
			}
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00009FE0 File Offset: 0x000081E0
		private void SetCell(ButtonCell cell, int index)
		{
			bool flag = index >= this.Result.Count;
			bool flag2 = !flag;
			if (flag2)
			{
				GameObject gameObject = this.Result[index];
				cell.Button.ButtonText.text = string.Concat(new string[]
				{
					"<color=cyan>",
					gameObject.name,
					"</color> (",
					UnityHelpers.GetTransformPath(gameObject.transform, true),
					")"
				});
			}
		}

		// Token: 0x06000189 RID: 393 RVA: 0x0000A063 File Offset: 0x00008263
		public override void SetDefaultSizeAndPosition()
		{
			base.SetDefaultSizeAndPosition();
			base.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500f);
			base.Rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 500f);
		}

		// Token: 0x0600018A RID: 394 RVA: 0x0000A094 File Offset: 0x00008294
		protected override void ConstructPanelContent()
		{
			this.dataHandler = new ButtonListHandler<GameObject, ButtonCell>(this.buttonScrollPool, new Func<List<GameObject>>(this.GetEntries), new Action<ButtonCell, int>(this.SetCell), new Func<GameObject, string, bool>(this.ShouldDisplayCell), new Action<int>(this.OnCellClicked));
			GameObject gameObject;
			GameObject gameObject2;
			this.buttonScrollPool = UIFactory.CreateScrollPool<ButtonCell>(base.ContentRoot, "ResultsList", out gameObject, out gameObject2, null);
			this.buttonScrollPool.Initialize(this.dataHandler, null);
			GameObject gameObject3 = gameObject;
			int? num = new int?(9999);
			UIFactory.SetLayoutElement(gameObject3, null, null, null, num, null, null, null);
		}

		// Token: 0x040000D5 RID: 213
		private ButtonListHandler<GameObject, ButtonCell> dataHandler;

		// Token: 0x040000D6 RID: 214
		private ScrollPool<ButtonCell> buttonScrollPool;
	}
}
