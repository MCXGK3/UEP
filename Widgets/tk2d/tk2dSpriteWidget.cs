using System;
using System.IO;
using System.Runtime.CompilerServices;
using GODump;
using HKTool;
using UnityEngine;
using UnityExplorer;
using UnityExplorer.Inspectors;
using UniverseLib.Utility;

namespace UnityExplorerPlus.Widgets.tk2d
{
	// Token: 0x02000033 RID: 51
	internal class tk2dSpriteWidget : DumpWidgetBase<tk2dSpriteWidget>
	{
		// Token: 0x060000DD RID: 221 RVA: 0x00006E08 File Offset: 0x00005008
		protected override void OnSave(string savePath)
		{
			bool flag = UnityHelpers.IsNullOrDestroyed(this.animation, true);
			if (flag)
			{
				ExplorerCore.LogWarning("Animation is null, maybe it was destroyed?");
			}
			else
			{
				Directory.CreateDirectory(savePath);
				bool value = ModBase<UnityExplorerPlus>.Instance.settings.useGODump.Value;
				if (value)
				{
					try
					{
						this.<OnSave>g__GODumpExtract|1_0(savePath);
						return;
					}
					catch (TypeLoadException)
					{
					}
					catch (MissingMemberException)
					{
					}
				}
				foreach (tk2dSpriteAnimationClip v in this.animation.clips)
				{
					string p = Path.Combine(savePath, v.name);
					Directory.CreateDirectory(p);
					int i = 0;
					foreach (tk2dSpriteAnimationFrame f in v.frames)
					{
						Texture2D tex = Utils.ExtractTk2dSprite(f.spriteCollection, f.spriteId);
						File.WriteAllBytes(Path.Combine(p, v.name + "-" + i.ToString() + ".png"), ImageConversion.EncodeToPNG(tex));
						Object.DestroyImmediate(tex);
						i++;
					}
				}
			}
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00006F4C File Offset: 0x0000514C
		public override void OnReturnToPool()
		{
			base.OnReturnToPool();
			this.animation = null;
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00006F60 File Offset: 0x00005160
		public override void OnBorrowed(object target, Type targetType, ReflectionInspector inspector)
		{
			base.OnBorrowed(target, targetType, inspector);
			tk2dSpriteAnimator anim = target as tk2dSpriteAnimator;
			bool flag = anim != null;
			string text;
			if (flag)
			{
				this.animation = anim.Library;
				bool flag2 = this.animation == null;
				if (flag2)
				{
					return;
				}
				text = anim.name;
			}
			else
			{
				this.animation = (tk2dSpriteAnimation)target;
				text = this.animation.name;
			}
			bool flag3 = string.IsNullOrEmpty(text);
			if (flag3)
			{
				text = "untitled";
			}
			base.SetDefaultPath("tk2d-" + text, "");
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x00006FFB File Offset: 0x000051FB
		[CompilerGenerated]
		private void <OnSave>g__GODumpExtract|1_0(string path)
		{
			GODump.Settings.SpriteBorder = false;
			global::CSC.GM.StartCoroutine(Dump.DumpSpriteInUExplorer(this.animation, path).SetIgnoreWait());
		}

		// Token: 0x04000081 RID: 129
		public tk2dSpriteAnimation animation;
	}
}
