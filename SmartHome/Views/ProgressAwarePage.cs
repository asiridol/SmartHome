using System;
using SmartHome.ViewModels;
using Xamarin.Forms;
namespace SmartHome.Views
{
	public abstract class ProgressAwarePage : ContentPage
	{
		public static BindableProperty ShowProgressProperty = BindableProperty.Create(nameof(ShowProgress), typeof(bool), typeof(ProgressAwarePage), null, BindingMode.OneWay);

		public bool ShowProgress
		{
			get
			{
				return (bool)GetValue(ShowProgressProperty);
			}
		}
	}

	public abstract class ProgressAwareViewModelPage<T> : ContentPage where T : ViewModelBase
	{
		public static BindableProperty ShowProgressProperty = BindableProperty.Create(nameof(ShowProgress), typeof(bool), typeof(ProgressAwarePage), null, BindingMode.OneWay);

		protected T ViewModel
		{
			get
			{
				return BindingContext as T;
			}
		}

		public bool ShowProgress
		{
			get
			{
				return (bool)GetValue(ShowProgressProperty);
			}
		}
	}
}
