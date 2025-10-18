using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace UnityExplorerPlus
{
	// Token: 0x02000066 RID: 102
	public static class TextureUtils
	{
		// Token: 0x06000240 RID: 576 RVA: 0x00008B10 File Offset: 0x00006D10
		public static Texture2D Cut(this Texture2D src, RectInt rect, TextureFormat? format = (TextureFormat)4)
		{
			Texture2D tex = new Texture2D(rect.width, rect.height, format ?? src.format, false);
			src.CopyTo(tex, new Vector2Int(rect.xMin, rect.yMin), Vector2Int.zero, rect.size, false, false);
			return tex;
		}

		// Token: 0x06000241 RID: 577 RVA: 0x00008B78 File Offset: 0x00006D78
		public static int GetPixelLength(TextureFormat format)
		{
			if ((int)format <= 62)
			{
				switch ((int)format)
				{
					case 1:
						return 1;
					case 2:
						return 2;
					case 3:
						return 3;
					case 4:
						return 4;
					case 5:
						return 4;
					case 6:
					case 8:
					case 10:
					case 11:
					case 12:
						break;
					case 7:
						return 3;
					case 9:
						return 2;
					case 13:
						return 2;
					case 14:
						return 4;
					case 15:
						return 2;
					case 16:
						return 4;
					case 17:
						return 8;
					case 18:
						return 4;
					case 19:
						return 8;
					case 20:
						return 16;
					default:
						if ((int)format == 62)
						{
							return 2;
						}
						break;
				}
			}
			else
			{
				if ((int)format == 63)
				{
					return 1;
				}
				if ((int)format == 74)
				{
					return 8;
				}
			}
			throw new NotSupportedException(string.Format("Not support texture format: {0}", format));
		}

		// Token: 0x06000242 RID: 578 RVA: 0x00008C58 File Offset: 0x00006E58
		public static void CopyTo(this Texture2D src, Texture2D dst, RectInt srcRect, RectInt dstRect)
		{
			if (srcRect.width == dstRect.width && srcRect.height == dstRect.height)
			{
				src.CopyTo(dst, srcRect.min, dstRect.min, srcRect.size, false, false);
				return;
			}
			Texture2D temp = src.Cut(srcRect, new TextureFormat?(dst.format));
			Texture2D texture2D = temp.Clone(dstRect.size, temp.format, null);
			texture2D.CopyTo(dst, Vector2Int.zero, dstRect.min, dstRect.size, false, false);
			UnityEngine.Object.DestroyImmediate(texture2D);
			UnityEngine.Object.DestroyImmediate(temp);
		}

		// Token: 0x06000243 RID: 579 RVA: 0x00008CF4 File Offset: 0x00006EF4
		public unsafe static void CopyTo(this Texture2D src, Texture2D dst, Vector2Int srcPosition, Vector2Int dstPosition, Vector2Int size, bool flipHorizontally = false, bool flipVertically = false)
		{
			if (!dst.isReadable)
			{
				throw new InvalidOperationException();
			}
			bool destroyTex = false;
			int len = TextureUtils.GetPixelLength(dst.format);
			try
			{
				if (src.format != dst.format || !src.isReadable)
				{
					src = src.CreateReadable(new TextureFormat?(dst.format), null);
					destroyTex = true;
				}
				NativeArray<byte> rawTextureData = src.GetRawTextureData<byte>();
				NativeArray<byte> dstRaw = dst.GetRawTextureData<byte>();
				byte* srcP = (byte*)NativeArrayUnsafeUtility.GetUnsafePtr<byte>(rawTextureData);
				byte* dstP = (byte*)NativeArrayUnsafeUtility.GetUnsafePtr<byte>(dstRaw);
				for (int y = 0; y < size.y; y++)
				{
					int srcStart = ((srcPosition.y + y) * src.width + srcPosition.x) * len;
					int dstStart = ((dstPosition.y + (flipVertically ? (size.y - y - 1) : y)) * dst.width + dstPosition.x) * len;
					if (flipHorizontally)
					{
						for (int x = size.x - 1; x >= 0; x--)
						{
							dstP[dstStart + x] = srcP[srcStart + x];
						}
					}
					else
					{
						UnsafeUtility.MemCpy((void*)(dstP + dstStart), (void*)(srcP + srcStart), (long)(size.x * len));
					}
				}
				dst.Apply(false, false);
			}
			finally
			{
				if (destroyTex)
				{
					UnityEngine.Object.DestroyImmediate(src);
				}
			}
		}

		// Token: 0x06000244 RID: 580 RVA: 0x00008E44 File Offset: 0x00007044
		public static Texture2D Clone(this Texture src, Vector2Int newSize, TextureFormat newFormat, Material material = null)
		{
			RenderTexture rt = src as RenderTexture;
			bool destroyTex;
			if (rt != null && material == null)
			{
				destroyTex = false;
			}
			else
			{
				rt = new RenderTexture(newSize.x, newSize.y, 0);
				if (material == null)
				{
					Graphics.Blit(src, rt);
				}
				else
				{
					Graphics.Blit(src, rt, material, 0);
				}
				destroyTex = true;
			}
			RenderTexture prev = RenderTexture.active;
			RenderTexture.active = rt;
			Texture2D tex = new Texture2D(newSize.x, newSize.y, newFormat, false);
			tex.ReadPixels(new Rect(0f, 0f, (float)tex.width, (float)tex.height), 0, 0, false);
			tex.Apply();
			if (prev == rt)
			{
				prev = null;
			}
			RenderTexture.active = prev;
			if (destroyTex)
			{
				rt.Release();
			}
			return tex;
		}

		// Token: 0x06000245 RID: 581 RVA: 0x00008F04 File Offset: 0x00007104
		public static Texture2D CreateReadable(this Texture2D src, TextureFormat? outputFormat = null, Material material = null)
		{
			return src.Clone(new Vector2Int(src.width, src.height), outputFormat ?? src.format, material);
		}
	}
}
