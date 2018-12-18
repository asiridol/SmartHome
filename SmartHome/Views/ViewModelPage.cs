using System;
using SmartHome.ViewModels;
using Xamarin.Forms;
using Prism.Ioc;

namespace SmartHome.Views
{
	public class ViewModelPage<T> : ContentPage where T : ViewModelBase
	{
		private T _viewModel;

		public T ViewModel
		{
			get
			{
				if (_viewModel == null)
				{
					_viewModel = Prism.PrismApplicationBase.Current.Container.Resolve<T>();
				}

				return _viewModel;
			}
		}

		public ViewModelPage()
		{
			BindingContext = ViewModel;
		}

		protected override void OnDisappearing()
		{
			_viewModel.Cleanup();
			_viewModel = null;

			base.OnDisappearing();
		}
	}
}
