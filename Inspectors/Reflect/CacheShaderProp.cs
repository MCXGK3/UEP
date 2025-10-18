using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityExplorer.CacheObject;
using UnityExplorer.Inspectors;

namespace UnityExplorerPlus.Inspectors.Reflect
{
	// Token: 0x0200005F RID: 95
	internal class CacheShaderProp : CacheProperty
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060001B6 RID: 438 RVA: 0x0000ADA4 File Offset: 0x00008FA4
		public override Type DeclaringType
		{
			get
			{
				return typeof(Material);
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x0000AC3C File Offset: 0x00008E3C
		public override bool IsStatic
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060001B8 RID: 440 RVA: 0x00002200 File Offset: 0x00000400
		public override bool ShouldAutoEvaluate
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x060001B9 RID: 441 RVA: 0x00002200 File Offset: 0x00000400
		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060001BA RID: 442 RVA: 0x0000ADB0 File Offset: 0x00008FB0
		public CacheShaderProp() : base(null)
		{
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000ADD0 File Offset: 0x00008FD0
		public override void SetInspectorOwner(ReflectionInspector inspector, MemberInfo member)
		{
			base.Owner = inspector;
			StringBuilder sb = new StringBuilder();
			sb.Append("<color=").Append("#55a38e").Append('>').Append("[Shader Prop]").Append("</color>").Append("<color=").Append("#c266ff").Append('>').Append(this.m_Name).Append("</color>");
			base.NameLabelText = sb.ToString();
			base.NameForFiltering = this.m_Name;
			base.NameLabelTextRaw = base.NameForFiltering;
		}

		// Token: 0x060001BC RID: 444 RVA: 0x0000AE74 File Offset: 0x00009074
		public void BindShaderProp(string name)
		{
			this.m_Name = name;
			this.m_Index = Shader.PropertyToID(name);
			base.FallbackType = typeof(object);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x0000AE9C File Offset: 0x0000909C
		protected override object TryEvaluate()
		{
			Material mat = (Material)base.DeclaringInstance;
			Shader shader = mat.shader;
			bool flag = shader == null;
			object result2;
			if (flag)
			{
				result2 = null;
			}
			else
			{
				int pid = shader.FindPropertyIndex(this.m_Name);
				ShaderPropertyType type = shader.GetPropertyType(pid);
				base.FallbackType = CacheShaderProp.shaderPropType[type];
				object result = CacheShaderProp.shaderPropGetter[type].Invoke(mat, new object[]
				{
					this.m_Index
				});
				bool flag2 = result == null && type == ShaderPropertyType.Texture;
				if (flag2)
				{
					string propertyTextureDefaultName = shader.GetPropertyTextureDefaultName(pid);
					if (!true)
					{
					}
					Texture2D texture2D;
					if (!(propertyTextureDefaultName == "white"))
					{
						if (!(propertyTextureDefaultName == "red"))
						{
							if (!(propertyTextureDefaultName == "black"))
							{
								if (!(propertyTextureDefaultName == "gray"))
								{
									texture2D = null;
								}
								else
								{
									texture2D = Texture2D.grayTexture;
								}
							}
							else
							{
								texture2D = Texture2D.blackTexture;
							}
						}
						else
						{
							texture2D = Texture2D.redTexture;
						}
					}
					else
					{
						texture2D = Texture2D.whiteTexture;
					}
					if (!true)
					{
					}
					result = texture2D;
				}
				result2 = result;
			}
			return result2;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x0000AFB4 File Offset: 0x000091B4
		protected override void TrySetValue(object value)
		{
			Material mat = (Material)base.DeclaringInstance;
			Shader shader = mat.shader;
			bool flag = shader == null;
			if (!flag)
			{
				CacheShaderProp.shaderPropSetter[shader.GetPropertyType(shader.FindPropertyIndex(this.m_Name))].Invoke(mat, new object[]
				{
					this.m_Index,
					value
				});
			}
		}

		// Token: 0x060001BF RID: 447 RVA: 0x0000B020 File Offset: 0x00009220
		// Note: this type is marked as 'beforefieldinit'.
		static CacheShaderProp()
		{
			Dictionary<ShaderPropertyType, Type> dictionary = new Dictionary<ShaderPropertyType, Type>();
			dictionary[ShaderPropertyType.Int] = typeof(int);
			dictionary[ShaderPropertyType.Float] = typeof(float);
			dictionary[ShaderPropertyType.Color] = typeof(Color);
			dictionary[ShaderPropertyType.Vector] = typeof(Vector4);
			dictionary[ShaderPropertyType.Texture] = typeof(Texture);
			dictionary[ShaderPropertyType.Range] = typeof(List<float>);
			CacheShaderProp.shaderPropType = dictionary;
		}

		// Token: 0x040000E6 RID: 230
		public static readonly Dictionary<ShaderPropertyType, MethodInfo> shaderPropGetter = Enumerable.ToDictionary<ValueTuple<ShaderPropertyType, string>, ShaderPropertyType, MethodInfo>(new ValueTuple<ShaderPropertyType, string>[]
		{
			new ValueTuple<ShaderPropertyType, string>(ShaderPropertyType.Texture, "GetTexture"),
			new ValueTuple<ShaderPropertyType, string>(ShaderPropertyType.Int, "GetInt"),
			new ValueTuple<ShaderPropertyType, string>(ShaderPropertyType.Float, "GetFloat"),
			new ValueTuple<ShaderPropertyType, string>(ShaderPropertyType.Color, "GetColor"),
			new ValueTuple<ShaderPropertyType, string>(ShaderPropertyType.Vector, "GetVector"),
			new ValueTuple<ShaderPropertyType, string>(ShaderPropertyType.Range, "GetFloatArray"),
		}, (ValueTuple<ShaderPropertyType, string> val) => val.Item1, (ValueTuple<ShaderPropertyType, string> val) => typeof(Material).GetMethod(val.Item2, new Type[]
		{
			typeof(int)
		}));

		// Token: 0x040000E7 RID: 231
		public static readonly Dictionary<ShaderPropertyType, MethodInfo> shaderPropSetter = Enumerable.ToDictionary<ValueTuple<ShaderPropertyType, string>, ShaderPropertyType, MethodInfo>(new ValueTuple<ShaderPropertyType, string>[]
		{
			new ValueTuple<ShaderPropertyType, string>(ShaderPropertyType.Texture, "SetTexture"),
			new ValueTuple<ShaderPropertyType, string>(ShaderPropertyType.Int, "SetInt"),
			new ValueTuple<ShaderPropertyType, string>(ShaderPropertyType.Float, "SetFloat"),
			new ValueTuple<ShaderPropertyType, string>(ShaderPropertyType.Color, "SetColor"),
			new ValueTuple<ShaderPropertyType, string>(ShaderPropertyType.Vector, "SetVector"),
			new ValueTuple<ShaderPropertyType, string>(ShaderPropertyType.Range, "SetFloatArray")
		}, (ValueTuple<ShaderPropertyType, string> val) => val.Item1, (ValueTuple<ShaderPropertyType, string> val) => Enumerable.First<MethodInfo>(typeof(Material).GetMethods(), (MethodInfo x) => x.Name == val.Item2 && x.GetParameters()[0].ParameterType == typeof(int)));

		// Token: 0x040000E8 RID: 232
		public static readonly Dictionary<ShaderPropertyType, Type> shaderPropType;

		// Token: 0x040000E9 RID: 233
		private string m_Name = "";

		// Token: 0x040000EA RID: 234
		private int m_Index = -1;
	}
}
