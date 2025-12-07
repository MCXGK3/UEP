using System;
using System.Collections.Generic;
using UnityEngine;


namespace UnityExplorerPlus.LineDrawing
{
	// Token: 0x0200004E RID: 78
	internal class LineRenderer2 : MonoBehaviour
	{
		// Token: 0x0600014C RID: 332 RVA: 0x00009640 File Offset: 0x00007840
		public static LineRenderer2 Instance;
		public LineRenderer2()
		{
			Instance = this;
		}
		private void OnGUI()
		{
			Event current = Event.current;
			bool flag = current == null || current.type != EventType.Repaint;
			if (!flag)
			{
				foreach (ILineProvider v in this.providers)
				{
					foreach (LineData i in v.Lines)
					{
						int prevDepth = GUI.depth;
						GUI.depth = i.Depth;
						Drawing.DrawLine(i.Start, i.End, i.Color, i.Width, false);
						GUI.depth = prevDepth;
					}
				}
			}
		}

		// Token: 0x040000C0 RID: 192
		public List<ILineProvider> providers = new List<ILineProvider>();
	}
}
