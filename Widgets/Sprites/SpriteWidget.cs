using System;
using System.Linq;
using HKTool.Reflection;
using HKTool.Utils;
using UnityEngine;
using UnityEngine.U2D;
using UnityExplorer;
using UnityExplorer.Inspectors;
using UnityExplorer.UI.Widgets;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace UnityExplorerPlus.Widgets.Sprites
{
	// Token: 0x02000036 RID: 54
	internal class SpriteWidget : Texture2DWidget
	{
		// Token: 0x060000EC RID: 236 RVA: 0x00007866 File Offset: 0x00005A66
		public SpriteWidget()
		{
			this.texWidget = ReflectionHelper.CreateReflectionObject(this);
		}

		// Token: 0x060000ED RID: 237 RVA: 0x0000787C File Offset: 0x00005A7C
		public override GameObject CreateContent(GameObject uiRoot)
		{
			GameObject r = base.CreateContent(uiRoot);
			this.atlasButton = UIFactory.CreateButton(base.UIRoot, "AtlasButton", "Inspect SpriteAtlas", new Color?(new Color(0.2f, 0.2f, 0.2f)));
			ButtonRef but = this.atlasButton;
			UIFactory.SetLayoutElement(but.Component.gameObject, new int?(160), new int?(25), null, null, null, null, null);
			but.Component.transform.SetSiblingIndex(3);
			but.Component.onClick.AddListener(delegate()
			{
				bool flag = this.atlas != null;
				if (flag)
				{
					InspectorManager.Inspect(this.atlas, null);
				}
				else
				{
					ExplorerCore.LogWarning("SpriteAtlas reference is null or destroyed!");
				}
			});
			this.gameObjectButton.Component.gameObject.SetActive(false);
			return r;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x0000796C File Offset: 0x00005B6C
		public override void OnBorrowed(object target, Type targetType, ReflectionInspector inspector)
		{
			Sprite sr = (Sprite)target;
			this.sprite = sr;
			this.atlasButton.Component.gameObject.SetActive(false);
			bool packed = sr.packed;
			if (packed)
			{
				this.atlas = Enumerable.FirstOrDefault<SpriteAtlas>(Resources.FindObjectsOfTypeAll<SpriteAtlas>(), (SpriteAtlas x) => x.CanBindTo(sr));
				bool flag = this.atlas != null;
				if (flag)
				{
					this.atlasButton.Component.gameObject.SetActive(true);
				}
			}
			this.viewTex = SpriteUtils.ExtractSprite(sr, true);
			this.viewTex.name = sr.name;
			base.OnBorrowed(this.viewTex, targetType, inspector);
			this.unityObject = (Object)target;
			this.nameInput.Text = this.unityObject.name;
			this.instanceIdInput.Text = this.unityObject.GetInstanceID().ToString();
			this.gameObjectButton.Component.gameObject.SetActive(false);
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00007A96 File Offset: 0x00005C96
		public override void OnReturnToPool()
		{
			base.OnReturnToPool();
			Object.Destroy(this.viewTex);
		}

		// Token: 0x04000086 RID: 134
		public Texture2D viewTex;

		// Token: 0x04000087 RID: 135
		private ReflectionObject texWidget;

		// Token: 0x04000088 RID: 136
		private Sprite sprite;

		// Token: 0x04000089 RID: 137
		private SpriteAtlas atlas;

		// Token: 0x0400008A RID: 138
		private ButtonRef atlasButton;
	}
}
