using System;
using System.Collections;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer.Inspectors;
using UnityExplorer.UI.Panels;
using UnityExplorer.UI.Widgets;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace UnityExplorerPlus.Widgets
{
	// Token: 0x0200002C RID: 44
	internal class RendererWidget : Texture2DWidget
	{
		// Token: 0x060000B1 RID: 177 RVA: 0x00005994 File Offset: 0x00003B94
		public override void OnBorrowed(object target, Type targetType, ReflectionInspector inspector)
		{
			this.renderer = (Renderer)target;
			UnityEngine.Object t = (UnityEngine.Object)target;
			this.ResetSize();
			target = RendererUtils.Render(this.renderer.gameObject, this.width, this.height);
			base.OnBorrowed(target, typeof(Texture2D), inspector);
			this.unityObject = t;
			this.nameInput.Text = this.unityObject.name;
			this.instanceIdInput.Text = this.unityObject.GetInstanceID().ToString();
			this.component = this.renderer;
			this.gameObjectButton.Component.gameObject.SetActive(true);
			this.refreshCor = RuntimeHelper.StartCoroutine(this.Refresh());
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00005A64 File Offset: 0x00003C64
		private void ResetSize()
		{
			this.width = (int)(this.renderer.bounds.size.x * 100f);
			this.height = (int)(this.renderer.bounds.size.y * 100f);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00005ABC File Offset: 0x00003CBC
		private void DoRefresh()
		{
			this.ResetSize();
			bool flag = this.prevTex != null;
			if (flag)
			{
				UnityEngine.Object.Destroy(this.prevTex);
			}
			this.prevTex = null;
			bool flag2 = this.width == 0 || this.height == 0 || this.width > 10000 || this.height > 10000;
			if (!flag2)
			{
				this.prevTex = RendererUtils.Render(this.renderer.gameObject, this.width, this.height, false);
				Traverse.Create(this).Field("texture").SetValue(this.prevTex);
				Traverse.Create(this).Method("SetupTextureViewer").GetValue();
				this.SetImageSize();
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00005B66 File Offset: 0x00003D66
		private IEnumerator Refresh()
		{
			yield return null;
			GameObject root = Traverse.Create(this).Field("textureViewerRoot").GetValue<GameObject>();
			for (; ; )
			{
				while (!root.activeInHierarchy || !this.autoRefresh)
				{
					yield return null;
				}
				this.DoRefresh();
				yield return new WaitForSecondsRealtime(0.05f);
			}
			yield break;
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00005B78 File Offset: 0x00003D78
		private void SetImageSize()
		{
			RectTransform imageRect = InspectorPanel.Instance.Rect;
			LayoutElement imageLayout = Traverse.Create(this).Field("imageLayout").GetValue<LayoutElement>();
			float rectWidth = imageRect.rect.width - 25f;
			float rectHeight = imageRect.rect.height - 196f;
			bool flag = (float)this.prevTex.width < rectWidth && (float)this.prevTex.height < rectHeight;
			if (flag)
			{
				imageLayout.minWidth = (float)this.prevTex.width;
				imageLayout.minHeight = (float)this.prevTex.height;
			}
			else
			{
				float viewWidthRatio = (float)((decimal)rectWidth / this.prevTex.width);
				float viewHeightRatio = (float)((decimal)rectHeight / this.prevTex.height);
				bool flag2 = viewWidthRatio < viewHeightRatio;
				if (flag2)
				{
					imageLayout.minWidth = (float)this.prevTex.width * viewWidthRatio;
					imageLayout.minHeight = (float)this.prevTex.height * viewWidthRatio;
				}
				else
				{
					imageLayout.minWidth = (float)this.prevTex.width * viewHeightRatio;
					imageLayout.minHeight = (float)this.prevTex.height * viewHeightRatio;
				}
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00005CCC File Offset: 0x00003ECC
		public override void OnReturnToPool()
		{
			bool flag = this.refreshCor != null;
			if (flag)
			{
				RuntimeHelper.StopCoroutine(this.refreshCor);
				this.refreshCor = null;
			}
			bool flag2 = this.prevTex != null;
			if (flag2)
			{
				UnityEngine.Object.Destroy(this.prevTex);
				this.prevTex = null;
			}
			base.OnReturnToPool();
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00005D28 File Offset: 0x00003F28
		public override GameObject CreateContent(GameObject uiRoot)
		{
			GameObject ret = base.CreateContent(uiRoot);
			GameObject saveRow = Traverse.Create(this).Field("textureViewerRoot").GetValue<GameObject>().transform.GetChild(0).gameObject;
			ButtonRef btn = UIFactory.CreateButton(saveRow, "RefreshBtn", "Refresh", new Color?(new Color(0.2f, 0.25f, 0.2f)));
			GameObject gameObject = btn.GameObject;
			int? num = new int?(25);
			UIFactory.SetLayoutElement(gameObject, new int?(100), num, new int?(99999), null, null, null, null);
			btn.Transform.SetAsLastSibling();
			ButtonRef buttonRef = btn;
			buttonRef.OnClick = (Action)Delegate.Combine(buttonRef.OnClick, new Action(delegate ()
			{
				this.DoRefresh();
			}));
			Toggle o_toggle;
			Text text;
			GameObject toggle = UIFactory.CreateToggle(saveRow, "AutoRefresh", out o_toggle, out text, default(Color), 20, 20);
			GameObject gameObject2 = toggle;
			num = new int?(25);
			UIFactory.SetLayoutElement(gameObject2, new int?(150), num, new int?(99999), null, null, null, null);
			toggle.transform.SetAsLastSibling();
			text.text = "Auto Refresh";
			o_toggle.onValueChanged.AddListener(delegate (bool val)
			{
				this.autoRefresh = val;
			});
			o_toggle.isOn = false;
			return ret;
		}

		// Token: 0x04000060 RID: 96
		public Renderer renderer;

		// Token: 0x04000061 RID: 97
		private Coroutine refreshCor;

		// Token: 0x04000062 RID: 98
		private int width = 2048;

		// Token: 0x04000063 RID: 99
		private int height = 1024;

		// Token: 0x04000064 RID: 100
		private Texture2D prevTex = null;

		// Token: 0x04000065 RID: 101
		private bool autoRefresh = false;
	}
}
