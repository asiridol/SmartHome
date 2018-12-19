using System;
using Foundation;

namespace SmartHome.iOS.Extensions
{
	public static class StringExtensions
	{
		public static NSString ToNSString(this string stringToConvert)
		{
			return new NSString(stringToConvert);
		}
	}
}
