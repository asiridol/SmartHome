using System;
using System.Globalization;
using Xamarin.Forms;

namespace SmartHome.Converters
{
	public class BoolToOnOffConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return "Offline";
			}

			if (value is bool booleanValue)
			{
				return booleanValue ? "On" : "Off";
			}

			return "Offline";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
