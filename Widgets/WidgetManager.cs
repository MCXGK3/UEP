using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.U2D;
using UnityExplorer.Inspectors;
using UnityExplorer.UI.Widgets;
using UnityExplorerPlus.Widgets.Sprites;
using UnityExplorerPlus.Widgets.tk2d;
using UniverseLib.UI.ObjectPool;
using HarmonyLib;

namespace UnityExplorerPlus.Widgets
{
	// Token: 0x0200002E RID: 46
	[HarmonyPatch]
	internal static class WidgetManager
	{
		// Token: 0x060000C1 RID: 193 RVA: 0x00005FDC File Offset: 0x000041DC
		static WidgetManager()
		{
			WidgetManager.RegisterType(typeof(tk2dSpriteCollectionData), typeof(Tk2dSpriteDefWidget));
			WidgetManager.RegisterType(typeof(Sprite), typeof(SpriteWidget));
			WidgetManager.RegisterType(typeof(SpriteAtlas), typeof(SpriteAtlasWidget));
			WidgetManager.RegisterType(typeof(tk2dSpriteAnimation), typeof(tk2dSpriteWidget));
			WidgetManager.RegisterType(typeof(tk2dSpriteAnimator), typeof(tk2dSpriteWidget));
			WidgetManager.RegisterType(typeof(tk2dSpriteAnimationClip), typeof(tk2dClipDumpWidget));
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x000060B0 File Offset: 0x000042B0
		public static void RegisterType(Type src, Type widget)
		{
			bool flag = !typeof(UnityObjectWidget).IsAssignableFrom(widget);
			if (!flag)
			{
				Type poolType = Pool.GetPool(widget).GetType();
				MethodInfo borrow = poolType.GetMethod("Borrow");
				WidgetManager.typeMap[src] = borrow;
			}
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0000610A File Offset: 0x0000430A

		[HarmonyPrefix]
		[HarmonyPatch(typeof(UnityObjectWidget), "GetUnityWidget", methodType: MethodType.Normal)]
		public static bool Init(object target, Type targetType, ReflectionInspector inspector, ref UnityObjectWidget __result)
		{

			try
			{
				MethodInfo borrow;
				bool flag = WidgetManager.typeMap.TryGetValue(targetType, out borrow);
				if (flag)
				{
					UnityObjectWidget r = (UnityObjectWidget)borrow.Invoke(null, []);
					r.OnBorrowed(target, targetType, inspector);
					__result = r;
					return false;
				}
				Renderer renderer = target as Renderer;
				bool flag2 = renderer != null && false;
				if (flag2)
				{
					__result = Helper.With<RendererWidget>(Pool<RendererWidget>.Borrow(), delegate (RendererWidget x)
					{
						x.OnBorrowed(target, targetType, inspector);
					});
					return false;
				}
			}
			catch (Exception e)
			{
				e.LogError();
			}
			return true;

		}

		// Token: 0x0400006A RID: 106
		private static Dictionary<Type, MethodInfo> typeMap = new Dictionary<Type, MethodInfo>();
	}
}
