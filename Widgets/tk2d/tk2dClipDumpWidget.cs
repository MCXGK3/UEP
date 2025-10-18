using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using UnityExplorer;
using UnityExplorer.Config;
using UnityExplorer.Inspectors;
using UnityExplorer.UI.Panels;
using UnityExplorer.UI.Widgets;
using UniverseLib;
using UniverseLib.UI;
using UniverseLib.UI.Models;

namespace UnityExplorerPlus.Widgets.tk2d
{
	// Token: 0x02000031 RID: 49
	internal class tk2dClipDumpWidget : Texture2DWidget
	{
		// Token: 0x060000C9 RID: 201 RVA: 0x00006250 File Offset: 0x00004450
		public override GameObject CreateContent(GameObject uiRoot)
		{
			GameObject ret = base.CreateContent(uiRoot);
			this.savePathInput.Transform.parent.gameObject.SetActive(false);
			GameObject textureViewerRoot = this.textureViewerRoot;
			string text = "PlayerWidget";
			bool flag = false;
			bool flag2 = false;
			bool flag3 = true;
			bool flag4 = true;
			int num = 5;
			Vector4 vector = default(Vector4);
			vector.x = 3f;
			vector.w = 3f;
			vector.y = 3f;
			vector.z = 3f;
			GameObject playerRow = UIFactory.CreateHorizontalGroup(textureViewerRoot, text, flag, flag2, flag3, flag4, num, vector, default(Color), null);
			playerRow.transform.SetAsFirstSibling();
			this.playStopButton = UIFactory.CreateButton(playerRow, "PlayerButton", "Play", new Color?(new Color(0.2f, 0.4f, 0.2f)));
			ButtonRef buttonRef = this.playStopButton;
			buttonRef.OnClick = (Action)Delegate.Combine(buttonRef.OnClick, new Action(this.OnPlayStopClicked));
			GameObject gameObject = this.playStopButton.GameObject;
			int? num2 = new int?(60);
			int? num3 = new int?(25);
			int? num4 = null;
			int? num5 = null;
			int? num6 = null;
			int? num7 = null;
			UIFactory.SetLayoutElement(gameObject, num2, num3, num4, num5, num6, num7, null);
			this.progressLabel = UIFactory.CreateLabel(playerRow, "ProgressLabel", "0 / 0", 3, default(Color), true, 14);
			GameObject gameObject2 = this.progressLabel.gameObject;
			num7 = new int?(9999);
			int? num8 = new int?(25);
			UIFactory.SetLayoutElement(gameObject2, null, num8, num7, null, null, null, null);
			GameObject saveRow = UIFactory.CreateHorizontalGroup(this.textureViewerRoot, "SpriteSaveRow", true, true, true, true, 2, new Vector4(2f, 2f, 2f, 2f), default(Color), null);
			saveRow.transform.SetSiblingIndex(1);
			GameObject gameObject3 = saveRow;
			num8 = new int?(30);
			num7 = new int?(9999);
			int? num9 = null;
			int? num10 = num8;
			int? num11 = num7;
			int? num12 = null;
			int? num13 = null;
			int? num14 = null;
			UIFactory.SetLayoutElement(gameObject3, num9, num10, num11, num12, num13, num14, null);
			Text saveLabel = UIFactory.CreateLabel(saveRow, "SpriteSaveLabel", "Save Clip:", 3, default(Color), true, 14);
			GameObject gameObject4 = saveLabel.gameObject;
			int? num15 = new int?(75);
			int? num16 = new int?(25);
			int? num17 = null;
			int? num18 = null;
			int? num19 = null;
			num7 = null;
			UIFactory.SetLayoutElement(gameObject4, num15, num16, num17, num18, num19, num7, null);
			this.savePlus = UIFactory.CreateInputField(saveRow, "SpriteSaveInput", "...");
			GameObject uiroot = this.savePlus.UIRoot;
			num7 = new int?(25);
			num8 = new int?(0);
			num14 = new int?(9999);
			UIFactory.SetLayoutElement(uiroot, null, num7, num14, num8, null, null, null);
			ButtonRef saveBtn = UIFactory.CreateButton(saveRow, "SaveButton", "Save Folder", new Color?(new Color(0.2f, 0.25f, 0.2f)));
			GameObject gameObject5 = saveBtn.Component.gameObject;
			num14 = new int?(25);
			int? num20 = new int?(100);
			int? num21 = num14;
			int? num22 = new int?(0);
			num8 = null;
			int? num23 = num8;
			num8 = null;
			int? num24 = num8;
			num8 = null;
			UIFactory.SetLayoutElement(gameObject5, num20, num21, num22, num23, num24, num8, null);
			ButtonRef buttonRef2 = saveBtn;
			buttonRef2.OnClick = (Action)Delegate.Combine(buttonRef2.OnClick, new Action(delegate()
			{
				string sp = this.savePlus.Text;
				bool flag5 = string.IsNullOrEmpty(sp);
				if (flag5)
				{
					ExplorerCore.LogWarning("Save path cannot be empty!");
				}
				else
				{
					Directory.CreateDirectory(sp);
					bool value = ModBase<UnityExplorerPlus>.Instance.settings.useGODump.Value;
					if (value)
					{
						try
						{
							this.<CreateContent>g__GODumpExtract|6_1(sp);
							return;
						}
						catch (TypeLoadException)
						{
						}
						catch (MissingMemberException)
						{
						}
					}
					int i = 0;
					foreach (tk2dSpriteAnimationFrame v in this.clip.frames)
					{
						Texture2D tex = Utils.ExtractTk2dSprite(v.spriteCollection, v.spriteId);
						File.WriteAllBytes(Path.Combine(sp, this.clip.name + "-" + i.ToString() + ".png"), ImageConversion.EncodeToPNG(tex));
						Object.DestroyImmediate(tex);
						i++;
					}
				}
			}));
			return ret;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00006644 File Offset: 0x00004844
		private static string GetLengthString(float seconds)
		{
			TimeSpan ts = TimeSpan.FromSeconds((double)seconds);
			StringBuilder sb = new StringBuilder();
			bool flag = ts.Hours > 0;
			if (flag)
			{
				sb.Append(string.Format("{0}:", ts.Hours));
			}
			sb.Append(string.Format("{0:00}:", ts.Minutes));
			sb.Append(string.Format("{0:00}:", ts.Seconds));
			sb.Append(string.Format("{0:000}", ts.Milliseconds));
			return sb.ToString();
		}

		// Token: 0x060000CB RID: 203 RVA: 0x000066EC File Offset: 0x000048EC
		private void ResetProgressLabel()
		{
			this.progressLabel.text = tk2dClipDumpWidget.GetLengthString(0f) + " / " + tk2dClipDumpWidget.GetLengthString((float)this.clip.frames.Length / this.clip.fps);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x0000673C File Offset: 0x0000493C
		private void OnPlayStopClicked()
		{
			bool flag = this.CurrentlyPlayingCoroutine != null;
			if (flag)
			{
				this.StopClip();
			}
			else
			{
				this.CurrentlyPlayingCoroutine = RuntimeHelper.StartCoroutine(this.PlayClipCoroutine());
			}
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00006774 File Offset: 0x00004974
		private void SetTex(tk2dSpriteCollectionData col, int id)
		{
			Texture2D tex = Utils.ExtractTk2dSprite(col, id);
			this.SetTex(tex);
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00006792 File Offset: 0x00004992
		private void SetTex(Texture2D tex)
		{
			this.texture = tex;
			this.SetupTextureViewer();
			this.SetImageSize(tex);
		}

		// Token: 0x060000CF RID: 207 RVA: 0x000067B0 File Offset: 0x000049B0
		private void SetImageSize(Texture2D prevTex)
		{
			RectTransform imageRect = InspectorPanel.Instance.Rect;
			LayoutElement imageLayout = this.imageLayout;
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

		// Token: 0x060000D0 RID: 208 RVA: 0x000068D1 File Offset: 0x00004AD1
		private IEnumerator PlayClipCoroutine()
		{
			this.playStopButton.ButtonText.text = "Stop Clip";
			float ws = 1f / this.clip.fps;
			bool flag2;
			do
			{
				yield return null;
				float st = Time.unscaledTime;
				Texture2D prevTex = null;
				foreach (tk2dSpriteAnimationFrame v in this.clip.frames)
				{
					bool flag = prevTex != null;
					if (flag)
					{
						Object.Destroy(prevTex);
					}
					prevTex = Utils.ExtractTk2dSprite(v.spriteCollection, v.spriteId);
					this.SetTex(prevTex);
					float fst = Time.unscaledTime;
					while (Time.unscaledTime - fst <= ws)
					{
						this.progressLabel.text = tk2dClipDumpWidget.GetLengthString(Time.unscaledTime - st) + " / " + tk2dClipDumpWidget.GetLengthString((float)this.clip.frames.Length / this.clip.fps);
						yield return null;
					}
					v = null;
				}
				tk2dSpriteAnimationFrame[] array = null;
				prevTex = null;
				tk2dSpriteAnimationClip.WrapMode wrapMode = this.clip.wrapMode;
				if (!true)
				{
				}
				tk2dSpriteAnimationClip.WrapMode wrapMode2 = wrapMode;
				flag2 = (wrapMode2 == null || wrapMode2 == 1 || wrapMode2 == 5);
				if (!true)
				{
				}
			}
			while (flag2);
			this.CurrentlyPlayingCoroutine = null;
			this.StopClip();
			yield break;
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x000068E0 File Offset: 0x00004AE0
		private void StopClip()
		{
			bool flag = this.CurrentlyPlayingCoroutine != null;
			if (flag)
			{
				RuntimeHelper.StopCoroutine(this.CurrentlyPlayingCoroutine);
			}
			this.CurrentlyPlayingCoroutine = null;
			this.playStopButton.ButtonText.text = "Play Clip";
			this.ResetProgressLabel();
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x0000692C File Offset: 0x00004B2C
		public override void OnReturnToPool()
		{
			base.OnReturnToPool();
			this.StopClip();
			foreach (Texture2D v in this.frames)
			{
				Object.Destroy(v);
			}
			this.frames.Clear();
			this.clip = null;
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x000069A8 File Offset: 0x00004BA8
		public override void OnBorrowed(object target, Type targetType, ReflectionInspector inspector)
		{
			this.clip = (tk2dSpriteAnimationClip)target;
			target = Utils.ExtractTk2dSprite(this.clip.frames[0].spriteCollection, this.clip.frames[0].spriteId);
			base.OnBorrowed(target, targetType, inspector);
			this.instanceIdInput.Component.gameObject.SetActive(false);
			this.unityObject = null;
			this.savePlus.Text = Path.Combine(ConfigManager.Default_Output_Path.Value, "tk2dSpriteAnimationClip-" + this.clip.name);
			this.ResetProgressLabel();
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00006B6C File Offset: 0x00004D6C
		[CompilerGenerated]
		private void <CreateContent>g__GODumpExtract|6_1(string path)
		{
			GODump.Settings.SpriteBorder = false;
			CoroutineHelper.StartCoroutine(Dump.DumpClip(this.clip, path, null, 0));
		}

		// Token: 0x04000070 RID: 112
		public tk2dSpriteAnimationClip clip;

		// Token: 0x04000071 RID: 113
		public ButtonRef playStopButton;

		// Token: 0x04000072 RID: 114
		public Text progressLabel;

		// Token: 0x04000073 RID: 115
		public InputFieldRef savePlus;

		// Token: 0x04000074 RID: 116
		public List<Texture2D> frames = new List<Texture2D>();

		// Token: 0x04000075 RID: 117
		private Coroutine CurrentlyPlayingCoroutine;
	}
}
