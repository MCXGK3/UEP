using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;
using UniverseLib.UI.Widgets.ScrollView;

namespace UnityExplorerPlus
{
	// Token: 0x02000015 RID: 21
	internal class ModPanel : CustomPanel, ICellPoolDataSource<ModCell>
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600004E RID: 78 RVA: 0x0000376F File Offset: 0x0000196F
		public override int MinHeight
		{
			get
			{
				return 200;
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600004F RID: 79 RVA: 0x00003776 File Offset: 0x00001976
		public override int MinWidth
		{
			get
			{
				return 360;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000050 RID: 80 RVA: 0x0000377D File Offset: 0x0000197D
		public override string Name
		{
			get
			{
				return "Mods";
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003784 File Offset: 0x00001984
		public ModPanel(UIBase owner) : base(owner)
		{
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000379C File Offset: 0x0000199C
		public override void ConstructPanelContent()
		{
			Transform title = UIRoot.transform.Find("Content").Find("TitleBar");
			// Transform title = GameObjectHelper.FindChildWithPath(this.UIRoot, new string[]
			// {
			// 	"Content",
			// 	"TitleBar"
			// }).transform;
			title.parent = this.UIRoot.transform;
			title.SetAsFirstSibling();
			UnityEngine.Object.Destroy(this.UIRoot.transform.Find("Content").gameObject);
			GameObject root;
			GameObject content;
			this.scrollPool = UIFactory.CreateScrollPool<ModCell>(this.UIRoot, "ModList", out root, out content, new Color?(new Color(0.11f, 0.11f, 0.11f)));
			UIFactory.SetLayoutElement(root, null, null, null, new int?(9999), null, null, null);
			UIFactory.SetLayoutElement(content, null, new int?(25), null, new int?(9999), null, null, null);
			content.GetComponent<VerticalLayoutGroup>().childControlHeight = false;
			this.Refresh();
			this.scrollPool.Initialize(this, null);
			this.Refresh();
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000053 RID: 83 RVA: 0x000038F4 File Offset: 0x00001AF4
		public override Vector2 DefaultAnchorMin
		{
			get
			{
				return new Vector2(0f, -0.1744325f);
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000054 RID: 84 RVA: 0x00003918 File Offset: 0x00001B18
		public override Vector2 DefaultAnchorMax
		{
			get
			{
				return new Vector2(0.3215279f, 0.4169654f);
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x0000393C File Offset: 0x00001B3C
		public void Refresh()
		{
			this.modCache.Clear();
			// foreach (KeyValuePair<Type, ModLoader.ModInstance> mod in ModLoader.ModInstanceTypeMap)
			// {
			// 	this.modCache.Add(new ValueTuple<IMod, Type>(mod.Value.Mod, mod.Key));
			// }
			this.scrollPool.Refresh(true, true);
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000039CC File Offset: 0x00001BCC
		public void OnCellBorrowed(ModCell cell)
		{
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000039D0 File Offset: 0x00001BD0
		public void SetCell(ModCell cell, int index)
		{
			bool flag = index < this.modCache.Count;
			if (flag)
			{
				// cell.BindMod(this.modCache[index]);
				cell.Enable();
			}
			else
			{
				cell.Disable();
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00003A17 File Offset: 0x00001C17
		public int ItemCount
		{
			get
			{
				return this.modCache.Count;
			}
		}

		// Token: 0x0400002B RID: 43
		public List<ValueTuple<MonoBehaviour, Type>> modCache = new List<ValueTuple<MonoBehaviour, Type>>();

		// Token: 0x0400002C RID: 44
		public ScrollPool<ModCell> scrollPool;
	}
}
