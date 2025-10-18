using System;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer.Inspectors;
using UnityExplorer.Inspectors.MouseInspectors;
using UnityExplorer.UI;
using UnityExplorer.UI.Panels;

namespace UnityExplorerPlus.Inspectors
{
	// Token: 0x0200005D RID: 93
	internal class WorldPositionPin : MouseInspectorBase
	{
		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060001A5 RID: 421 RVA: 0x00009CC0 File Offset: 0x00007EC0
		public static Text objNameLabel
		{
			get
			{
				return Traverse.Create(MouseInspector.Instance).Field("objNameLabel").GetValue<Text>();
			}
		}

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x00009CCD File Offset: 0x00007ECD
		public static Text objPathLabel
		{
			get
			{
				return Traverse.Create(MouseInspector.Instance).Field("objPathLabel").GetValue<Text>();
			}
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x000039CC File Offset: 0x00001BCC
		public override void OnBeginMouseInspect()
		{
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x000039CC File Offset: 0x00001BCC
		public override void ClearHitData()
		{
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x000039CC File Offset: 0x00001BCC
		public override void OnEndInspect()
		{
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000AB88 File Offset: 0x00008D88
		public override void UpdateMouseInspect(Vector2 _)
		{
			WorldPositionPin.objNameLabel.text = "<b>World Position: </b><color=cyan>" + CameraSwitcher.GetCurrentMousePosition().ToString() + "</color>";
			WorldPositionPin.objPathLabel.text = "";
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000ABD4 File Offset: 0x00008DD4
		public override void OnSelectMouseInspect()
		{
			Vector3 pos = Input.mousePosition;
			pos.z = CameraSwitcher.GetCurrentCamera().WorldToScreenPoint(Vector3.zero).z;
			pos = CameraSwitcher.GetCurrentCamera().ScreenToWorldPoint(pos);
			ClipboardPanel.Copy(pos);
			UEUIManager.SetPanelActive(UnityExplorer.UI.UIManager.Panels.Clipboard, true);
		}
	}
}
