using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityExplorerPlus
{

	public static class Helper
	{
		// Token: 0x0600021E RID: 542 RVA: 0x0000818F File Offset: 0x0000638F
		public static T MemberwiseClone<T>(this T self)
		{
			return (T)((object)self.MemberwiseClone());
		}

		// Token: 0x0600021F RID: 543 RVA: 0x000081A2 File Offset: 0x000063A2
		public static T With<T>(this T self, Action<T> action)
		{
			action(self);
			return self;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x000081AC File Offset: 0x000063AC
		public static T With<T>(this T self, Helper.WithDelegate<T> action)
		{
			action(ref self);
			return self;
		}

		// Token: 0x06000221 RID: 545 RVA: 0x000081B8 File Offset: 0x000063B8
		public static TValue TryGetOrAddValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> init)
		{
			TValue result;
			if (!dict.TryGetValue(key, out result))
			{
				result = init();
				dict.Add(key, result);
			}
			return result;
		}

		// Token: 0x0200005F RID: 95
		// (Invoke) Token: 0x06000223 RID: 547

		public delegate void WithDelegate<T>(ref T self);
	}
}
