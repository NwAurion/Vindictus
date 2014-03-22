
using System;

namespace VindictusCraftingCostCalculator.Utils
{
	public static class StringExtensions
	{
		public static bool IsEmpty(this String s)
		{
			if (s == null || s == "")
				return true;

			return false;
		}

		public static bool IsNotEmpty(this String s)
		{
			if (s == null || s == "")
				return false;

			return true;
		}
	}
}
