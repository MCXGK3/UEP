using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UEP;
using UnityEngine;

namespace UnityExplorerPlus
{
	// Token: 0x02000023 RID: 35
	public static class Utils
	{
		// Token: 0x06000080 RID: 128 RVA: 0x000049EC File Offset: 0x00002BEC
		public static void NoThrow(Action action)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				UEPPlugin.logger.LogError(e);
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00004A24 File Offset: 0x00002C24
		public static void NextFrame(Action action)
		{
			static IEnumerator NextFrameCoro(Action action)
			{
				yield return null;
				action();
			}
			UEPPlugin.Instance.StartCoroutine(NextFrameCoro(action));
		}



		// Token: 0x06000082 RID: 130 RVA: 0x00004A51 File Offset: 0x00002C51
		// public static IEnumerator SetIgnoreWait(this IEnumerator coroutine)
		// {
		// 	FlatEnumerator flat = new FlatEnumerator(coroutine);
		// 	while (flat.MoveNext())
		// 	{
		// 		bool flag = flat.Current is WaitForSeconds || flat.Current is WaitForSecondsRealtime;
		// 		if (!flag)
		// 		{
		// 			yield return flat.Current;
		// 		}
		// 	}
		// 	yield break;
		// }

		// Token: 0x06000083 RID: 131 RVA: 0x00004A60 File Offset: 0x00002C60
		public static Texture2D ExtractTk2dSprite(tk2dSpriteCollectionData def, int id)
		{
			Dictionary<string, ValueTuple<Texture2D, int>> text = new Dictionary<string, ValueTuple<Texture2D, int>>();
			for (int i = 0; i < Utils.CheckCount; i++)
			{
				Texture2D tex = SpriteUtils.ExtractTk2dSprite(def, id, false);
				using (MD5 md5 = MD5.Create())
				{
					string m5 = BitConverter.ToString(md5.ComputeHash(ImageConversion.EncodeToPNG(tex)));
					ValueTuple<Texture2D, int> t;
					bool flag = text.TryGetValue(m5, out t);
					if (flag)
					{
						UnityEngine.Object.DestroyImmediate(tex);
					}
					else
					{
						t = new ValueTuple<Texture2D, int>(tex, 0);
					}
					t.Item2++;
					text[m5] = t;
				}
			}
			Texture2D maxTex = null;
			int maxCount = -1;
			foreach (KeyValuePair<string, ValueTuple<Texture2D, int>> keyValuePair in text)
			{
				string text2;
				ValueTuple<Texture2D, int> valueTuple;
				keyValuePair.Deconstruct(out text2, out valueTuple);
				ValueTuple<Texture2D, int> valueTuple2 = valueTuple;
				Texture2D tex2 = valueTuple2.Item1;
				int c = valueTuple2.Item2;
				bool flag2 = c > maxCount;
				if (flag2)
				{
					maxCount = c;
					maxTex = tex2;
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(tex2);
				}
			}
			tk2dSpriteDefinition sdef = def.spriteDefinitions[id];
			Bounds trimedB = sdef.GetBounds();
			Bounds untrimedB = sdef.GetUntrimmedBounds();
			float pixelPerUnitX = (float)maxTex.width / trimedB.size.x;
			float pixelPerUnitY = (float)maxTex.height / trimedB.size.y;
			Texture2D otex = TextureUtils.Clone(Texture2D.redTexture, new Vector2Int((int)(untrimedB.size.x * pixelPerUnitX), (int)(untrimedB.size.y * pixelPerUnitY)), (TextureFormat)4, null);
			int offsetX = (int)(Mathf.Abs(trimedB.min.x - untrimedB.min.x) * pixelPerUnitX);
			int offsetY = (int)(Mathf.Abs(trimedB.min.y - untrimedB.min.y) * pixelPerUnitY);
			TextureUtils.CopyTo(maxTex, otex, new RectInt(0, 0, maxTex.width, maxTex.height), new RectInt(offsetX, offsetY, maxTex.width, maxTex.height));
			UnityEngine.Object.DestroyImmediate(maxTex);
			return otex;
		}

		// Token: 0x04000051 RID: 81
		public static int CheckCount = 5;
	}
}
