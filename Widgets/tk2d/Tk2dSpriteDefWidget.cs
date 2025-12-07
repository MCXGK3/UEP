using System;
using System.IO;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer;
using UnityExplorer.Config;
using UnityExplorer.Inspectors;
using UnityExplorer.UI.Panels;
using UnityExplorer.UI.Widgets;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.Utility;

namespace UnityExplorerPlus.Widgets.tk2d
{
	// Token: 0x02000034 RID: 52
	internal class Tk2dSpriteDefWidget : Texture2DWidget
	{
		// Token: 0x060000E2 RID: 226 RVA: 0x00007028 File Offset: 0x00005228
		public override void OnBorrowed(object target, Type targetType, ReflectionInspector inspector)
		{
			this.collection = (tk2dSpriteCollectionData)target;
			target = Utils.ExtractTk2dSprite(this.collection, 0);
			this.savePlus.Text = Path.Combine(UnityExplorer.Config.ConfigManager.Default_Output_Path.Value, "tk2dSpriteCollectionData-" + this.collection.name);
			base.OnBorrowed(target, typeof(Texture2D), inspector);
			this.unityObject = this.collection;
			this.instanceIdInput.Text = this.collection.GetInstanceID().ToString();
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000070C0 File Offset: 0x000052C0
		private void SetImageSize(Texture2D prevTex)
		{
			RectTransform imageRect = InspectorPanel.Instance.Rect;
			LayoutElement imageLayout = Traverse.Create(this).Field("imageLayout").GetValue<LayoutElement>();
			float rectWidth = imageRect.rect.width - 25f;
			float rectHeight = imageRect.rect.height - 196f;
			bool flag = (float)prevTex.width < rectWidth && (float)prevTex.height < rectHeight;
			if (flag)
			{
				imageLayout.minWidth = (float)prevTex.width;
				imageLayout.minHeight = (float)prevTex.height;
			}
			else
			{
				float viewWidthRatio = (float)((decimal)rectWidth / prevTex.width);
				float viewHeightRatio = (float)((decimal)rectHeight / prevTex.height);
				bool flag2 = viewWidthRatio < viewHeightRatio;
				if (flag2)
				{
					imageLayout.minWidth = (float)prevTex.width * viewWidthRatio;
					imageLayout.minHeight = (float)prevTex.height * viewWidthRatio;
				}
				else
				{
					imageLayout.minWidth = (float)prevTex.width * viewHeightRatio;
					imageLayout.minHeight = (float)prevTex.height * viewHeightRatio;
				}
			}
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x000071E4 File Offset: 0x000053E4
		public override GameObject CreateContent(GameObject uiRoot)
		{
			GameObject ret = base.CreateContent(uiRoot);
			GameObject saveRow = UIFactory.CreateHorizontalGroup(Traverse.Create(this).Field("textureViewerRoot").GetValue<GameObject>(), "SpriteSaveRow", true, true, true, true, 2, new Vector4(2f, 2f, 2f, 2f), default(Color), null);
			saveRow.transform.SetSiblingIndex(1);
			GameObject gameObject = saveRow;
			int? num = new int?(30);
			int? num2 = new int?(9999);
			int? num3 = null;
			int? num4 = num;
			int? num5 = num2;
			int? num6 = null;
			int? num7 = null;
			int? num8 = null;
			UIFactory.SetLayoutElement(gameObject, num3, num4, num5, num6, num7, num8, null);
			Text saveLabel = UIFactory.CreateLabel(saveRow, "SpriteSaveLabel", "Save All:", TextAnchor.MiddleLeft, default(Color), true, 14);
			GameObject gameObject2 = saveLabel.gameObject;
			int? num9 = new int?(75);
			int? num10 = new int?(25);
			num2 = null;
			int? num11 = num2;
			num2 = null;
			int? num12 = num2;
			num2 = null;
			int? num13 = num2;
			num2 = null;
			UIFactory.SetLayoutElement(gameObject2, num9, num10, num11, num12, num13, num2, null);
			this.savePlus = UIFactory.CreateInputField(saveRow, "SpriteSaveInput", "...");
			GameObject uiroot = this.savePlus.UIRoot;
			num2 = new int?(25);
			num = new int?(0);
			num8 = new int?(9999);
			UIFactory.SetLayoutElement(uiroot, null, num2, num8, num, null, null, null);
			ButtonRef saveBtn = UIFactory.CreateButton(saveRow, "SaveButton", "Save Folder", new Color?(new Color(0.2f, 0.25f, 0.2f)));
			GameObject gameObject3 = saveBtn.Component.gameObject;
			num8 = new int?(25);
			int? num14 = new int?(100);
			int? num15 = num8;
			int? num16 = new int?(0);
			num = null;
			int? num17 = num;
			num = null;
			int? num18 = num;
			num = null;
			UIFactory.SetLayoutElement(gameObject3, num14, num15, num16, num17, num18, num, null);
			ButtonRef buttonRef = saveBtn;
			buttonRef.OnClick = (Action)Delegate.Combine(buttonRef.OnClick, new Action(delegate ()
			{
				string sp = this.savePlus.Text;
				bool flag = string.IsNullOrEmpty(sp);
				if (flag)
				{
					ExplorerCore.LogWarning("Save path cannot be empty!");
				}
				else
				{
					Directory.CreateDirectory(sp);
					for (int i = 0; i < this.collection.spriteDefinitions.Length; i++)
					{
						string name = this.collection.spriteDefinitions[i].name;
						Texture2D tex = Utils.ExtractTk2dSprite(this.collection, i);
						File.WriteAllBytes(Path.Combine(sp, name + ".png"), ImageConversion.EncodeToPNG(tex));
						UnityEngine.Object.Destroy(tex);
					}
				}
			}));
			GameObject idRow = UIFactory.CreateHorizontalGroup(Traverse.Create(this).Field("textureViewerRoot").GetValue<GameObject>().transform.GetChild(0).gameObject, "SpriteIDRow", true, true, true, true, 2, new Vector4(2f, 2f, 2f, 2f), default(Color), null);
			idRow.transform.SetAsFirstSibling();
			GameObject gameObject4 = idRow;
			num8 = new int?(30);
			num = new int?(9999);
			num2 = null;
			int? num19 = num2;
			int? num20 = num8;
			int? num21 = num;
			num2 = null;
			int? num22 = num2;
			num2 = null;
			int? num23 = num2;
			num2 = null;
			UIFactory.SetLayoutElement(gameObject4, num19, num20, num21, num22, num23, num2, null);
			Text spriteIdLabel = UIFactory.CreateLabel(idRow, "SpriteIDLabel", "SpriteId:", TextAnchor.MiddleLeft, default(Color), true, 14);
			GameObject gameObject5 = spriteIdLabel.gameObject;
			int? num24 = new int?(75);
			int? num25 = new int?(25);
			num = null;
			int? num26 = num;
			num = null;
			int? num27 = num;
			num = null;
			int? num28 = num;
			num = null;
			UIFactory.SetLayoutElement(gameObject5, num24, num25, num26, num27, num28, num, null);
			this.spriteIdInput = UIFactory.CreateInputField(idRow, "SpriteIDInput", "e.g. 0");
			GameObject uiroot2 = this.spriteIdInput.UIRoot;
			num = new int?(25);
			UIFactory.SetLayoutElement(uiroot2, new int?(75), num, new int?(250), null, null, null, null);
			UnityHelpers.GetOnEndEdit(this.spriteIdInput.Component).AddListener(delegate (string val)
			{
				int id;
				bool flag = !int.TryParse(val, out id);
				if (!flag)
				{
					bool flag2 = id < 0;
					if (flag2)
					{
						id = 0;
						this.spriteIdInput.Text = "0";
					}
					bool flag3 = id >= this.collection.spriteDefinitions.Length;
					if (flag3)
					{
						id = this.collection.spriteDefinitions.Length - 1;
						this.spriteIdInput.Text = id.ToString();
					}
					Texture2D tex = Utils.ExtractTk2dSprite(this.collection, id);
					tex.name = this.collection.spriteDefinitions[id].name;
					Traverse.Create(this).Field("texture").SetValue(tex);
					Traverse.Create(this).Method("SetupTextureViewer").GetValue();
					this.SetImageSize(tex);
				}
			});
			return ret;
		}

		// Token: 0x04000082 RID: 130
		public InputFieldRef spriteIdInput;

		// Token: 0x04000083 RID: 131
		public InputFieldRef savePlus;

		// Token: 0x04000084 RID: 132
		public tk2dSpriteCollectionData collection;
	}
}
