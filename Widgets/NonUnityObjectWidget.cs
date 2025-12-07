using System;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer.Inspectors;
using UnityExplorer.UI.Widgets;
using UniverseLib.UI;

namespace UnityExplorerPlus.Widgets
{
	// Token: 0x0200002B RID: 43
	public class NonUnityObjectWidget : UnityObjectWidget
	{
		// Token: 0x060000AD RID: 173 RVA: 0x0000584C File Offset: 0x00003A4C
		public override GameObject CreateContent(GameObject uiRoot)
		{
			base.UIRoot = UIFactory.CreateUIObject("UnityObjectRow", uiRoot, default(Vector2));
			UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(base.UIRoot, new bool?(false), new bool?(false), new bool?(true), new bool?(true), new int?(5), null, null, null, null, null);
			UIFactory.SetLayoutElement(base.UIRoot, null, new int?(25), new int?(9999), new int?(0), null, null, null);
			return base.UIRoot;
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000039CC File Offset: 0x00001BCC
		public override void Update()
		{
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00005920 File Offset: 0x00003B20
		public override void OnBorrowed(object target, Type targetType, ReflectionInspector inspector)
		{
			this.owner = inspector;
			bool flag = base.UIRoot == null;
			if (flag)
			{
				this.CreateContent(inspector.UIRoot);
			}
			else
			{
				base.UIRoot.transform.SetParent(inspector.UIRoot.transform);
			}
			base.UIRoot.transform.SetSiblingIndex(inspector.UIRoot.transform.childCount - 2);
		}
	}
}
