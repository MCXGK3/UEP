using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityExplorerPlus

{
	// Token: 0x02000064 RID: 100
	public static class SpriteUtils
	{
		// Token: 0x06000236 RID: 566 RVA: 0x00008622 File Offset: 0x00006822
		public static Texture2D ExtractSprite(Sprite sprite)
		{
			return SpriteUtils.ExtractSprite(sprite, false);
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000862C File Offset: 0x0000682C
		public static Texture2D ExtractSprite(Sprite sprite, bool fixborder)
		{
			if (SpriteUtils.cacheSpriteR == null)
			{
				GameObject gameObject = new GameObject("Sprite Renderer");
				gameObject.transform.position = new Vector3(0f, 0f, 55555f);
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				SpriteUtils.cacheSpriteR = gameObject.AddComponent<SpriteRenderer>();
			}
			SpriteUtils.cacheSpriteR.sprite = sprite;
			int width = (int)(sprite.bounds.size.x * sprite.pixelsPerUnit);
			int height = (int)(sprite.bounds.size.y * sprite.pixelsPerUnit);
			width = ((width > 0) ? width : 1);
			height = ((height > 0) ? height : 1);
			SpriteUtils.cacheSpriteR.gameObject.SetActive(true);
			Texture2D tex2d = SpriteUtils.cacheSpriteR.gameObject.Render(width, height);
			SpriteUtils.cacheSpriteR.gameObject.SetActive(false);
			if (fixborder)
			{
				Vector2Int origSize = new(width, height);
				Vector2Int offset = Vector2Int.zero;
				Vector2 pivot = sprite.pivot / new Vector2((float)width, (float)height);
				Vector2 delta = new Vector2(0.5f, 0.5f) - pivot;
				if (delta.x > 0f)
				{
					offset.x = Mathf.RoundToInt(delta.x * 2f) * width;
					origSize.x = width + offset.x;
				}
				else
				{
					origSize.x = width + Mathf.RoundToInt(-delta.x * 2f) * width;
				}
				if (delta.y > 0f)
				{
					offset.y = Mathf.RoundToInt(delta.y * 2f) * height;
					origSize.y = height + offset.y;
				}
				else
				{
					origSize.y = height + Mathf.RoundToInt(-delta.y * 2f) * height;
				}
				Texture2D otex = Texture2D.redTexture.Clone(origSize, (TextureFormat)4, null);
				tex2d.CopyTo(otex, new RectInt(0, 0, tex2d.width, tex2d.height), new RectInt(offset.x, offset.y, tex2d.width, tex2d.height));
				UnityEngine.Object.DestroyImmediate(tex2d);
				tex2d = otex;
			}
			return tex2d;
		}

		// Token: 0x06000238 RID: 568 RVA: 0x00008850 File Offset: 0x00006A50
		public static Texture2D ExtractTk2dSprite(tk2dSpriteCollectionData def, int id)
		{
			return SpriteUtils.ExtractTk2dSprite(def, id, false);
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000885C File Offset: 0x00006A5C
		public static Texture2D ExtractTk2dSprite(tk2dSpriteCollectionData def, int id, bool fixborder)
		{
			tk2dSpriteDefinition sdef = def.spriteDefinitions[id];
			if (SpriteUtils.cacheTk2d == null)
			{
				GameObject gameObject = new GameObject("Tk2d Renderer", new Type[]
				{
					typeof(MeshRenderer),
					typeof(MeshFilter)
				});
				gameObject.transform.position = new Vector3(0f, 0f, 55555f);
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				SpriteUtils.cacheTk2d = tk2dSprite.AddComponent(gameObject, def, id);
			}
			SpriteUtils.cacheTk2d.SetSprite(def, id);
			int width = (int)((Enumerable.Max<Vector2>(sdef.uvs, (Vector2 x) => x.x) - Enumerable.Min<Vector2>(sdef.uvs, (Vector2 x) => x.x)) * (float)sdef.material.mainTexture.width) + 1;
			int height = (int)((Enumerable.Max<Vector2>(sdef.uvs, (Vector2 x) => x.y) - Enumerable.Min<Vector2>(sdef.uvs, (Vector2 x) => x.y)) * (float)sdef.material.mainTexture.height) + 1;
			if (sdef.flipped == (tk2dSpriteDefinition.FlipMode)1)
			{
				int num = width;
				width = height;
				height = num;
			}
			SpriteUtils.cacheTk2d.gameObject.SetActive(true);
			Texture2D tex2d = SpriteUtils.cacheTk2d.gameObject.Render(width, height);
			SpriteUtils.cacheTk2d.gameObject.SetActive(false);
			if (fixborder)
			{
				Bounds trimedB = sdef.GetBounds();
				Bounds untrimedB = sdef.GetUntrimmedBounds();
				float pixelPerUnitX = (float)tex2d.width / trimedB.size.x;
				float pixelPerUnitY = (float)tex2d.height / trimedB.size.y;
				Texture2D otex = Texture2D.redTexture.Clone(new Vector2Int((int)(untrimedB.size.x * pixelPerUnitX), (int)(untrimedB.size.y * pixelPerUnitY)), (TextureFormat)4, null);
				int offsetX = (int)(Mathf.Abs(trimedB.min.x - untrimedB.min.x) * pixelPerUnitX);
				int offsetY = (int)(Mathf.Abs(trimedB.min.y - untrimedB.min.y) * pixelPerUnitY);
				tex2d.CopyTo(otex, new RectInt(0, 0, tex2d.width, tex2d.height), new RectInt(offsetX, offsetY, tex2d.width, tex2d.height));
				UnityEngine.Object.DestroyImmediate(tex2d);
				tex2d = otex;
			}
			return tex2d;
		}

		// Token: 0x040000F8 RID: 248
		private static tk2dSprite cacheTk2d;

		// Token: 0x040000F9 RID: 249
		private static SpriteRenderer cacheSpriteR;
	}
}
