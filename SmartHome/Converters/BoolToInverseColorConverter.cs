using System;
using System.Globalization;
using Xamarin.Forms;
namespace SmartHome.Converters
{
	public class BoolToInverseColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
			{
				return Color.White;
			}

			if (value is bool booleanValue)
			{
				return booleanValue ? Color.Black : Color.White;
			}

			return Color.White;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
