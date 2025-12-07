using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer.Inspectors;
using UniverseLib.UI;

namespace UnityExplorerPlus.Inspectors
{
	[HarmonyPatch(typeof(MouseInspector), "UpdateInspect", MethodType.Normal)]
	public class Patch_MouseInspector_UpdateInspect
	{
		public static bool Prefix(MouseInspector __instance)
		{
			return true;
		}
		public static void Postfix(MouseInspector __instance)
		{
			bool mouseButtonDown = Input.GetMouseButtonDown(1);
			if (mouseButtonDown)
			{
				CameraSwitcher.SwitchCamera();
			}
			CameraSwitcher.disableCameraSwitch = MouseInspector.Mode == null || MouseInspector.Mode == MouseInspectMode.UI;
			CameraSwitcher.switchText.gameObject.SetActive(!CameraSwitcher.disableCameraSwitch);
			bool flag = !CameraSwitcher.disableCameraSwitch;
			if (flag)
			{
				Traverse.Create(MouseInspector.Instance).Field("mousePosLabel").GetValue<Text>().text = "<color=grey>Mouse Position:</color> " + CameraSwitcher.GetCurrentMousePosition().ToString();
			}
		}
	}
	[HarmonyPatch(typeof(MouseInspector), "ConstructPanelContent", MethodType.Normal)]
	public class Patch_MouseInspector_ConstructPanelContent
	{
		public static bool Prefix(MouseInspector __instance)
		{
			return true;
		}
		public static void Postfix(MouseInspector __instance)
		{

			GameObject inspect = __instance.ContentRoot.transform.Find("InspectContent").gameObject;
			inspect.transform.GetChild(0).gameObject.SetActive(false);
			CameraSwitcher.switchText = UIFactory.CreateLabel(inspect, "CameraSwitcherText", "Press the right mouse button to switch the camera", TextAnchor.MiddleCenter, default(Color), true, 14);
			CameraSwitcher.switchText.horizontalOverflow = HorizontalWrapMode.Overflow;
			CameraSwitcher.switchText.transform.SetSiblingIndex(0);
		}
	}
	// Token: 0x0200004F RID: 79
	internal static class CameraSwitcher
	{
		// Token: 0x0600014E RID: 334 RVA: 0x00009748 File Offset: 0x00007948
		public static void Init()
		{

		}


		// Token: 0x06000151 RID: 337 RVA: 0x000098B8 File Offset: 0x00007AB8
		private static void RefreshCameras()
		{
			CameraSwitcher.lastUpdate = Time.realtimeSinceStartup;
			CameraSwitcher.cameras.RemoveAll((Camera x) => x == null);
			foreach (Camera cam in Camera.allCameras)
			{
				bool flag = !CameraSwitcher.cameras.Contains(cam);
				if (flag)
				{
					CameraSwitcher.cameras.Add(cam);
				}
			}
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00009938 File Offset: 0x00007B38
		private static void RefreshText()
		{
			bool flag = CameraSwitcher.curCamera == null;
			string t;
			if (flag)
			{
				t = "No camera";
			}
			else
			{
				t = CameraSwitcher.curCamera.name;
				bool flag2 = CameraSwitcher.curCamera == Camera.main;
				if (flag2)
				{
					t += "<color=green>(Main Camera)</color>";
				}
			}
			CameraSwitcher.switchText.text = "<b>Press the right mouse button to switch the camera.</b> Current: " + t;
		}

		// Token: 0x06000153 RID: 339 RVA: 0x000099A4 File Offset: 0x00007BA4
		internal static void SwitchCamera()
		{
			CameraSwitcher.RefreshCameras();
			int index = CameraSwitcher.cameras.IndexOf(CameraSwitcher.GetCurrentCamera()) + 1;
			bool flag = index == 0;
			if (flag)
			{
				CameraSwitcher.curCamera = Camera.main;
			}
			else
			{
				bool flag2 = CameraSwitcher.cameras.Count <= index;
				if (flag2)
				{
					index = 0;
				}
				CameraSwitcher.curCamera = CameraSwitcher.cameras[index];
			}
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00009A0C File Offset: 0x00007C0C
		public static Camera GetCurrentCamera()
		{
			bool flag = Time.realtimeSinceStartup > CameraSwitcher.lastUpdate + 0.5f;
			if (flag)
			{
				CameraSwitcher.RefreshCameras();
			}
			bool flag2 = CameraSwitcher.curCamera == null;
			if (flag2)
			{
				CameraSwitcher.curCamera = Camera.main;
			}
			CameraSwitcher.RefreshText();
			return CameraSwitcher.curCamera;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00009A64 File Offset: 0x00007C64
		public static Vector2 GetCurrentMousePosition()
		{
			Camera cam = CameraSwitcher.GetCurrentCamera();
			Vector3 mousePos = Input.mousePosition;
			mousePos.z = cam.WorldToScreenPoint(Vector3.zero).z;
			return cam.ScreenToWorldPoint(mousePos);
		}

		// Token: 0x040000C1 RID: 193
		private static List<Camera> cameras = new List<Camera>();

		// Token: 0x040000C2 RID: 194
		private static Camera curCamera;

		// Token: 0x040000C3 RID: 195
		private static float lastUpdate;

		// Token: 0x040000C4 RID: 196
		internal static bool disableCameraSwitch = false;

		// Token: 0x040000C5 RID: 197
		internal static Text switchText;

	}
}
