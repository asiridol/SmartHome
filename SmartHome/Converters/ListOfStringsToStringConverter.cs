using System;
using System.Globalization;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Linq;
namespace SmartHome.Converters
{
	public class ListOfStringsToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is List<string> listOfStrings)
			{
				if (!listOfStrings.Any())
				{
					return string.Empty;
				}

				if (listOfStrings.Count > 1)
				{
					return $"{listOfStrings.Count} devices";
				}
				else
				{
					return "1 device";
				}
			}

			return string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
