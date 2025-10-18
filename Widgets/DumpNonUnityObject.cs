using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityExplorer;
using UnityExplorer.Config;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace UnityExplorerPlus.Widgets
{
	// Token: 0x02000027 RID: 39
	public abstract class DumpNonUnityObject<T> : NonUnityObjectWidget where T : DumpNonUnityObject<T>
	{
		// Token: 0x06000093 RID: 147 RVA: 0x00004DF0 File Offset: 0x00002FF0
		public override GameObject CreateContent(GameObject uiRoot)
		{
			GameObject result = base.CreateContent(uiRoot);
			this.savePathInput = UIFactory.CreateInputField(base.UIRoot, "SaveInput", "...");
			UIFactory.SetLayoutElement(this.savePathInput.UIRoot, new int?(100), new int?(25), new int?(9999), null, null, null, null);
			ButtonRef buttonRef = UIFactory.CreateButton(base.UIRoot, "SaveButton", "Dump", new Color?(new Color(0.2f, 0.25f, 0.2f)));
			buttonRef.Component.onClick.AddListener(new UnityAction(this.OnClickSave));
			UIFactory.SetLayoutElement(buttonRef.Component.gameObject, new int?(100), new int?(25), new int?(0), null, null, null, null);
			return result;
		}

		// Token: 0x06000094 RID: 148
		protected abstract void OnSave(string savePath);

		// Token: 0x06000095 RID: 149 RVA: 0x00004F0C File Offset: 0x0000310C
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

		// Token: 0x06000096 RID: 150 RVA: 0x00004F90 File Offset: 0x00003190
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

		// Token: 0x0400005A RID: 90
		public InputFieldRef savePathInput;

		// Token: 0x0400005B RID: 91
		private string ext;
	}
}
