using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityExplorer.CacheObject;
using UnityExplorer.Inspectors;

namespace UnityExplorerPlus.Inspectors.Reflect
{
	// Token: 0x0200005E RID: 94
	internal class CacheShaderKeywords : CacheProperty
	{
		// Token: 0x060001AD RID: 429 RVA: 0x0000AC31 File Offset: 0x00008E31
		public CacheShaderKeywords() : base(null)
		{
		}

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060001AE RID: 430 RVA: 0x0000AC3C File Offset: 0x00008E3C
		public override bool HasArguments
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060001AF RID: 431 RVA: 0x00002200 File Offset: 0x00000400
		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060001B0 RID: 432 RVA: 0x0000AC3C File Offset: 0x00008E3C
		public override bool IsStatic
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x00002200 File Offset: 0x00000400
		public override bool ShouldAutoEvaluate
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000AC40 File Offset: 0x00008E40
		public override void SetInspectorOwner(ReflectionInspector inspector, MemberInfo member)
		{
			base.Owner = inspector;
			StringBuilder sb = new StringBuilder();
			sb.Append("<color=").Append("#55a38e").Append('>').Append("[Shader Keywords]").Append("</color>").Append("<color=").Append("#c266ff").Append('>').Append(this.m_Name).Append("</color>");
			base.NameLabelText = sb.ToString();
			base.NameForFiltering = this.m_Name;
			base.NameLabelTextRaw = base.NameForFiltering;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x0000ACE4 File Offset: 0x00008EE4
		public void BindShaderKeyword(string name)
		{
			this.m_Name = name;
			base.FallbackType = typeof(bool);
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000AD00 File Offset: 0x00008F00
		protected override object TryEvaluate()
		{
			Material mat = (Material)base.DeclaringInstance;
			Shader shader = mat.shader;
			bool flag = shader == null;
			object result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = mat.IsKeywordEnabled(this.m_Name);
			}
			return result;
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x0000AD48 File Offset: 0x00008F48
		protected override void TrySetValue(object value)
		{
			Material mat = (Material)base.DeclaringInstance;
			Shader shader = mat.shader;
			bool val = (bool)value;
			bool flag = shader == null;
			if (!flag)
			{
				bool flag2 = val;
				if (flag2)
				{
					mat.EnableKeyword(this.m_Name);
				}
				else
				{
					mat.DisableKeyword(this.m_Name);
				}
			}
		}

		// Token: 0x040000E5 RID: 229
		private string m_Name;
	}
}
