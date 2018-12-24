using System;
using Xamarin.Forms;
namespace SmartHome.Controls
{
	public class SelectableViewCell : ViewCell
	{
		public static readonly BindableProperty SelectedBackgroundColorProperty =
		BindableProperty.Create("SelectedBackgroundColor",
								typeof(Color),
								typeof(SelectableViewCell),
								Color.Default);

		public Color SelectedBackgroundColor
		{
			get { return (Color)GetValue(SelectedBackgroundColorProperty); }
			set { SetValue(SelectedBackgroundColorProperty, value); }
		}
	}
}
