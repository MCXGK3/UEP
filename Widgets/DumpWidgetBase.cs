using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityExplorer;
using UnityExplorer.Config;
using UnityExplorer.Inspectors;
using UnityExplorer.UI.Widgets;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.ObjectPool;

namespace UnityExplorerPlus.Widgets
{
	// Token: 0x02000028 RID: 40
	public abstract class DumpWidgetBase<T> : UnityObjectWidget where T : DumpWidgetBase<T>
	{
		// Token: 0x06000098 RID: 152 RVA: 0x00004FF4 File Offset: 0x000031F4
		public override GameObject CreateContent(GameObject uiRoot)
		{
			GameObject result = base.CreateContent(uiRoot);
			ButtonRef toggleButton = UIFactory.CreateButton(base.UIRoot, "DumpWidgetToggleButton", "Dump", new Color?(new Color(0.2f, 0.3f, 0.2f)));
			toggleButton.Transform.SetSiblingIndex(0);
			toggleButton.Component.onClick.AddListener(delegate()
			{
				this.panel.SetActive(!this.panel.activeSelf);
			});
			UIFactory.SetLayoutElement(toggleButton.Component.gameObject, new int?(100), new int?(25), null, null, null, null, null);
			GameObject parent = UIFactory.CreateHorizontalGroup(uiRoot, "Save Row", false, false, true, true, 2, new Vector4(2f, 2f, 2f, 2f), new Color(0.1f, 0.1f, 0.1f), null);
			UIFactory.SetLayoutElement(parent, null, null, new int?(9999), new int?(35), null, null, null);
			this.panel = parent;
			this.savePathInput = UIFactory.CreateInputField(parent, "SaveInput", "...");
			UIFactory.SetLayoutElement(this.savePathInput.UIRoot, new int?(100), new int?(25), new int?(9999), null, null, null, null);
			ButtonRef buttonRef = UIFactory.CreateButton(parent, "SaveButton", "Dump", new Color?(new Color(0.2f, 0.25f, 0.2f)));
			buttonRef.Component.onClick.AddListener(new UnityAction(this.OnClickSave));
			UIFactory.SetLayoutElement(buttonRef.Component.gameObject, new int?(100), new int?(25), new int?(0), null, null, null, null);
			parent.SetActive(false);
			return result;
		}

		// Token: 0x06000099 RID: 153
		protected abstract void OnSave(string savePath);

		// Token: 0x0600009A RID: 154 RVA: 0x00005258 File Offset: 0x00003458
		private void OnClickSave()
		{
			string sp = this.savePathInput.Text;
			bool flag = string.IsNullOrEmpty(this.savePathInput.Text);
			if (flag)
			{
				ExplorerCore.LogWarning("Save path cannot be empty!");
			}
			else
			{
				bool flag2 = !string.IsNullOrEmpty(this.ext) && !sp.EndsWith("." + this.ext, StringComparison.OrdinalIgnoreCase);
				if (flag2)
				{
					sp = sp + "." + this.ext;
				}
				this.OnSave(sp);
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000052DC File Offset: 0x000034DC
		public override void OnReturnToPool()
		{
			base.OnReturnToPool();
			this.panel.transform.SetParent(Pool<T>.Instance.InactiveHolder.transform);
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00005308 File Offset: 0x00003508
		protected void SetDefaultPath(string fn, string ext = "")
		{
			bool flag = string.IsNullOrEmpty(fn);
			if (flag)
			{
				fn = "untitled";
			}
			this.ext = ext;
			this.savePathInput.Text = Path.Combine(ConfigManager.Default_Output_Path.Value, string.IsNullOrEmpty(ext) ? fn : (fn + "." + ext));
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005364 File Offset: 0x00003564
		public override void OnBorrowed(object target, Type targetType, ReflectionInspector inspector)
		{
			base.OnBorrowed(target, targetType, inspector);
			this.panel.transform.SetParent(inspector.UIRoot.transform);
			this.panel.transform.SetSiblingIndex(inspector.UIRoot.transform.childCount - 2);
		}

		// Token: 0x0400005C RID: 92
		public InputFieldRef savePathInput;

		// Token: 0x0400005D RID: 93
		public GameObject panel;

		// Token: 0x0400005E RID: 94
		private string ext;
	}
}
