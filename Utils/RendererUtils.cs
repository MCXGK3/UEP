using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityExplorerPlus
{
	public static class RendererUtils
	{
		// Token: 0x06000228 RID: 552 RVA: 0x000081E8 File Offset: 0x000063E8
		private static void PrepareCamera()
		{
			GameObject gameObject = new GameObject("HKTool Cache Camera");
			RendererUtils.cacheCamera = gameObject.AddComponent<Camera>();
			RendererUtils.cacheCamera.orthographic = true;
			RendererUtils.cacheCamera.clearFlags = (CameraClearFlags)4;
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000821C File Offset: 0x0000641C
		public static void BuildCamera(Bounds bounds)
		{
			if (RendererUtils.cacheCamera == null)
			{
				RendererUtils.PrepareCamera();
			}
			RendererUtils.cacheCamera.orthographicSize = bounds.size.y / 2f;
			RendererUtils.cacheCamera.aspect = bounds.size.x / bounds.size.y;
			RendererUtils.cacheCamera.transform.position = bounds.center.With(delegate (ref Vector3 x)
			{
				x.z = bounds.min.z - 1f;
			});
		}

		// Token: 0x0600022A RID: 554 RVA: 0x000082BE File Offset: 0x000064BE
		public static Texture2D Render(this GameObject go, int width, int height)
		{
			return go.Render(width, height, false);
		}

		// Token: 0x0600022B RID: 555 RVA: 0x000082CC File Offset: 0x000064CC
		public static Texture2D Render(this GameObject go, int width, int height, bool includeChildren)
		{
			width = ((width > 0) ? width : 1);
			height = ((height > 0) ? height : 1);
			Renderer render = go.GetComponent<Renderer>();
			if (render == null)
			{
				return null;
			}
			Vector3 origPos = go.transform.position;
			bool origE = render.enabled;
			Renderer[] enabled = Enumerable.ToArray<Renderer>(Enumerable.Where<Renderer>(go.GetComponentsInChildren<Renderer>(false), (Renderer x) => x.enabled && x.gameObject.activeInHierarchy));
			if (!includeChildren)
			{
				go.transform.position = go.transform.position.With(delegate (ref Vector3 x)
				{
					x.z = -999999f;
				});
				Renderer[] array = enabled;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = false;
				}
			}
			render.enabled = true;
			Bounds bounds = default(Bounds);
			if (!includeChildren)
			{
				bounds = render.bounds;
			}
			else
			{
				bounds.min = new Vector3(Enumerable.Min(Enumerable.Select<Renderer, float>(enabled, (Renderer x) => x.bounds.min.x)), Enumerable.Min(Enumerable.Select<Renderer, float>(enabled, (Renderer x) => x.bounds.min.y)), render.bounds.min.z);
				bounds.max = new Vector3(Enumerable.Max(Enumerable.Select<Renderer, float>(enabled, (Renderer x) => x.bounds.max.x)), Enumerable.Max(Enumerable.Select<Renderer, float>(enabled, (Renderer x) => x.bounds.max.y)), render.bounds.max.z);
			}
			RendererUtils.BuildCamera(bounds);
			RenderTexture rtex = new RenderTexture(width, height, 0);
			rtex.Create();
			RendererUtils.cacheCamera.targetTexture = rtex;
			RendererUtils.cacheCamera.Render();
			go.transform.position = origPos;
			render.enabled = origE;
			if (!includeChildren)
			{
				Renderer[] array = enabled;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = true;
				}
			}
			Texture2D texture2D = new Texture2D(width, height);
			RenderTexture prev = RenderTexture.active;
			RenderTexture.active = rtex;
			texture2D.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
			texture2D.Apply();
			RenderTexture.active = prev;
			rtex.Release();
			return texture2D;
		}

		// Token: 0x040000EF RID: 239
		private static Camera cacheCamera;
	}
}
