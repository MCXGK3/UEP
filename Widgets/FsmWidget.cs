using System;
using System.Collections.Generic;
using System.IO;
using HKTool;
using HKTool.Utils;
using HutongGames.PlayMaker;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityExplorer;
using UnityExplorer.Inspectors;
using UnityExplorerPlus.FSMViewer;
using UnityExplorerPlus.Patch;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.Utility;

namespace UnityExplorerPlus.Widgets
{
	// Token: 0x02000029 RID: 41
	internal class FsmWidget : DumpWidgetBase<FsmWidget>
	{
		// Token: 0x060000A0 RID: 160 RVA: 0x000053E4 File Offset: 0x000035E4
		protected override void OnSave(string savePath)
		{
			bool flag = UnityHelpers.IsNullOrDestroyed(this.fsm, true);
			if (flag)
			{
				ExplorerCore.LogWarning("PlayMakerFSM is null, maybe it was destroyed?");
			}
			else
			{
				File.WriteAllText(savePath, this.GetFsmJson(this.fsm));
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00005424 File Offset: 0x00003624
		private string GetFsmJson(PlayMakerFSM fsm)
		{
			Fsm f = fsm.Fsm;
			foreach (FsmState v in f.States)
			{
				v.actionData = (v.ActionData ?? new ActionData());
				v.SaveActions();
			}
			JToken token = JToken.Parse(JsonConvert.SerializeObject(f, 1, new JsonSerializerSettings
			{
				ContractResolver = new UnityContractResolver(),
				Converters = new List<JsonConverter>
				{
					new FsmWidget.UnityObjectConverter()
				},
				ReferenceLoopHandling = 1
			}));
			token["goName"] = fsm.gameObject.name;
			token["goPath"] = GameObjectHelper.GetPath(fsm.gameObject);
			token["fsmId"] = fsm.GetInstanceID();
			return token.ToString(1, Array.Empty<JsonConverter>());
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00005516 File Offset: 0x00003716
		public override void OnReturnToPool()
		{
			base.OnReturnToPool();
			this.fsm = null;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00005528 File Offset: 0x00003728
		public override GameObject CreateContent(GameObject uiRoot)
		{
			GameObject result = base.CreateContent(uiRoot);
			ButtonRef btn = UIFactory.CreateButton(base.UIRoot, "OpenInFSMViewer", "Open", new Color?(new Color(0.2f, 0.3f, 0.2f)));
			btn.Transform.SetSiblingIndex(0);
			btn.Component.onClick.AddListener(delegate()
			{
				FSMViewerManager.OpenJsonFsm(this.GetFsmJson(this.fsm));
			});
			UIFactory.SetLayoutElement(btn.Component.gameObject, new int?(100), new int?(25), null, null, null, null, null);
			return result;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x000055EC File Offset: 0x000037EC
		public override void OnBorrowed(object target, Type targetType, ReflectionInspector inspector)
		{
			base.OnBorrowed(target, targetType, inspector);
			this.fsm = (PlayMakerFSM)target;
			base.SetDefaultPath(this.fsm.gameObject.name + "-" + this.fsm.Fsm.Name, "json");
		}

		// Token: 0x0400005F RID: 95
		public PlayMakerFSM fsm;

		// Token: 0x0200002A RID: 42
		private class UnityObjectConverter : JsonConverter
		{
			// Token: 0x17000019 RID: 25
			// (get) Token: 0x060000A7 RID: 167 RVA: 0x00002200 File Offset: 0x00000400
			public override bool CanRead
			{
				get
				{
					return true;
				}
			}

			// Token: 0x1700001A RID: 26
			// (get) Token: 0x060000A8 RID: 168 RVA: 0x00002200 File Offset: 0x00000400
			public override bool CanWrite
			{
				get
				{
					return true;
				}
			}

			// Token: 0x060000A9 RID: 169 RVA: 0x00005664 File Offset: 0x00003864
			public override bool CanConvert(Type objectType)
			{
				return typeof(Object).IsAssignableFrom(objectType);
			}

			// Token: 0x060000AA RID: 170 RVA: 0x00005678 File Offset: 0x00003878
			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				return existingValue;
			}

			// Token: 0x060000AB RID: 171 RVA: 0x0000568C File Offset: 0x0000388C
			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				Object uobj = (Object)value;
				string name = uobj.name;
				string file = "";
				writer.WriteStartObject();
				GameObject go = uobj as GameObject;
				bool flag = go != null;
				if (flag)
				{
					name = GameObjectHelper.GetPath(go);
					ModBase<UnityExplorerPlus>.Instance.LogFine(name + " (GO)");
					bool flag2 = go.scene.IsValid();
					if (flag2)
					{
						file = go.scene.name;
					}
					else
					{
						file = PatchGameObjectControls.GetPrefabFile(go);
					}
				}
				else
				{
					Component c = uobj as Component;
					bool flag3 = c != null;
					if (flag3)
					{
						ModBase<UnityExplorerPlus>.Instance.LogFine(uobj.name + " (C)");
						go = c.gameObject;
						name = GameObjectHelper.GetPath(go) + " (" + c.GetType().FullName + ")";
						ModBase<UnityExplorerPlus>.Instance.LogFine(name + " (GO)");
						bool flag4 = go.scene.IsValid();
						if (flag4)
						{
							file = go.scene.name;
						}
						else
						{
							file = PatchGameObjectControls.GetPrefabFile(go);
						}
					}
				}
				writer.WritePropertyName("objName");
				writer.WriteValue(name);
				writer.WritePropertyName("objFile");
				writer.WriteValue(file);
				writer.WritePropertyName("objId");
				writer.WriteValue(uobj.GetInstanceID());
				writer.WritePropertyName("objType");
				writer.WriteValue(value.GetType().FullName);
				writer.WriteEndObject();
				ModBase<UnityExplorerPlus>.Instance.LogFine(name + " (" + file + ")");
			}
		}
	}
}
