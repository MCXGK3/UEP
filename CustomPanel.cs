using System;
using UnityExplorer.UI;
using UnityExplorer.UI.Panels;
using UniverseLib.UI;

namespace UnityExplorerPlus
{
	// Token: 0x02000009 RID: 9
	internal abstract class CustomPanel : UEPanel
	{
		// Token: 0x06000011 RID: 17 RVA: 0x000021DA File Offset: 0x000003DA
		protected CustomPanel(UIBase owner) : base(owner)
		{
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000012 RID: 18 RVA: 0x000021F8 File Offset: 0x000003F8
		public override UEUIManager.Panels PanelType
		{
			get
			{
				return this.panelType;
			}
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000013 RID: 19 RVA: 0x00002200 File Offset: 0x00000400
		public override bool ShouldSaveActiveState
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000014 RID: 20 RVA: 0x00002200 File Offset: 0x00000400
		public override bool ShowByDefault
		{
			get
			{
				return true;
			}
		}

		// Token: 0x04000005 RID: 5
		internal UEUIManager.Panels panelType = (UEUIManager.Panels)CustomPanel.id++;

		// Token: 0x04000006 RID: 6
		private static int id = 100;
	}
}
