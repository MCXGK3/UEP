using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace UnityExplorerPlus.LineDrawing
{

	public class LineData : IEquatable<LineData>
	{
		// Token: 0x06000135 RID: 309 RVA: 0x0000928F File Offset: 0x0000748F
		public LineData(Vector2 Start, Vector2 End, Color Color, int Depth, float Width = 0.7f)
		{
			this.Start = Start;
			this.End = End;
			this.Color = Color;
			this.Depth = Depth;
			this.Width = Width;
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000136 RID: 310 RVA: 0x000092BD File Offset: 0x000074BD
		[CompilerGenerated]
		protected virtual Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(LineData);
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000137 RID: 311 RVA: 0x000092C9 File Offset: 0x000074C9
		// (set) Token: 0x06000138 RID: 312 RVA: 0x000092D1 File Offset: 0x000074D1
		public Vector2 Start { get; set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000139 RID: 313 RVA: 0x000092DA File Offset: 0x000074DA
		// (set) Token: 0x0600013A RID: 314 RVA: 0x000092E2 File Offset: 0x000074E2
		public Vector2 End { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600013B RID: 315 RVA: 0x000092EB File Offset: 0x000074EB
		// (set) Token: 0x0600013C RID: 316 RVA: 0x000092F3 File Offset: 0x000074F3
		public Color Color { get; set; }

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x0600013D RID: 317 RVA: 0x000092FC File Offset: 0x000074FC
		// (set) Token: 0x0600013E RID: 318 RVA: 0x00009304 File Offset: 0x00007504
		public int Depth { get; set; }

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600013F RID: 319 RVA: 0x0000930D File Offset: 0x0000750D
		// (set) Token: 0x06000140 RID: 320 RVA: 0x00009315 File Offset: 0x00007515
		public float Width { get; set; }

		// Token: 0x06000141 RID: 321 RVA: 0x00009320 File Offset: 0x00007520
		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("LineData");
			stringBuilder.Append(" { ");
			if (this.PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000936C File Offset: 0x0000756C
		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("Start = ");
			builder.Append(this.Start.ToString());
			builder.Append(", End = ");
			builder.Append(this.End.ToString());
			builder.Append(", Color = ");
			builder.Append(this.Color.ToString());
			builder.Append(", Depth = ");
			builder.Append(this.Depth.ToString());
			builder.Append(", Width = ");
			builder.Append(this.Width.ToString());
			return true;
		}

		// Token: 0x06000143 RID: 323 RVA: 0x00009442 File Offset: 0x00007642
		[CompilerGenerated]
		public static bool operator !=(LineData left, LineData right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(LineData left, LineData right)
		{
			return left == right || (left != null && left.Equals(right));
		}


		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			return this.Equals(obj as LineData);
		}

		// Token: 0x06000149 RID: 329 RVA: 0x000095B8 File Offset: 0x000077B8
		[CompilerGenerated]
		protected LineData(LineData original)
		{
			this.Start = original.Start;
			this.End = original.End;
			this.Color = original.Color;
			this.Depth = original.Depth;
			this.Width = original.Width;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00009608 File Offset: 0x00007808
		[CompilerGenerated]
		public void Deconstruct(out Vector2 Start, out Vector2 End, out Color Color, out int Depth, out float Width)
		{
			Start = this.Start;
			End = this.End;
			Color = this.Color;
			Depth = this.Depth;
			Width = this.Width;
		}

		public bool Equals(LineData other)
		{
			return this == other || (other != null && this.EqualityContract == other.EqualityContract &&
			EqualityComparer<Vector2>.Default.Equals(this.Start, other.Start) &&
			EqualityComparer<Vector2>.Default.Equals(this.End, other.End) &&
			EqualityComparer<Color>.Default.Equals(this.Color, other.Color) &&
			EqualityComparer<int>.Default.Equals(this.Depth, other.Depth) &&
			EqualityComparer<float>.Default.Equals(this.Width, other.Width));

		}
	}
}
