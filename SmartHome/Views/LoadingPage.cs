using System;
using Xamarin.Forms;
namespace SmartHome.Views
{
	public class LoadingPage : ProgressAwarePage
	{
		protected override void OnAppearing()
		{
			base.OnAppearing();
			BackgroundColor = new Color(178, 0, 0, 25.5);
		}
	}
}
