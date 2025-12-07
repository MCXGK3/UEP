using System;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UnityExplorer;
using UnityExplorer.Inspectors;
using UniverseLib.Utility;

namespace UnityExplorerPlus.Widgets.Sprites
{
	// Token: 0x02000035 RID: 53
	internal class SpriteAtlasWidget : DumpWidgetBase<SpriteAtlasWidget>
	{
		// Token: 0x060000E8 RID: 232 RVA: 0x00007734 File Offset: 0x00005934
		protected override void OnSave(string savePath)
		{
			bool flag = UnityHelpers.IsNullOrDestroyed(this.atlas, true);
			if (flag)
			{
				ExplorerCore.LogWarning("SpriteAtlas is null, maybe it was destroyed?");
			}
			else
			{
				Directory.CreateDirectory(savePath);
				Sprite[] sr = new Sprite[this.atlas.spriteCount];
				this.atlas.GetSprites(sr);
				foreach (Sprite v in sr)
				{
					string p = Path.Combine(savePath, v.name.Replace("(Clone)", "") + ".png");
					Texture2D tex = SpriteUtils.ExtractSprite(v, true);
					File.WriteAllBytes(p, ImageConversion.EncodeToPNG(tex));
					UnityEngine.Object.DestroyImmediate(tex);
					UnityEngine.Object.DestroyImmediate(v);
				}
			}
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x000077F1 File Offset: 0x000059F1
		public override void OnReturnToPool()
		{
			base.OnReturnToPool();
			this.atlas = null;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00007804 File Offset: 0x00005A04
		public override void OnBorrowed(object target, Type targetType, ReflectionInspector inspector)
		{
			base.OnBorrowed(target, targetType, inspector);
			this.atlas = (SpriteAtlas)target;
			string text = this.atlas.name;
			bool flag = string.IsNullOrEmpty(text);
			if (flag)
			{
				text = "untitled";
			}
			base.SetDefaultPath("Atlas-" + text, "");
		}

		// Token: 0x04000085 RID: 133
		public SpriteAtlas atlas;
	}
}
