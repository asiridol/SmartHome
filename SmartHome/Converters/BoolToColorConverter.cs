using System;
using System.Globalization;
using Xamarin.Forms;

namespace SmartHome.Converters
{
	public class BoolToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return Color.Red;
			}

			if (value is bool booleanValue)
			{
				return booleanValue ? Color.Yellow : Color.Green;
			}

			return Color.Red;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
