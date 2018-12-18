using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace SmartHome.ViewModels
{
	public class MainPageViewModel : ViewModelBase
	{
		public MainPageViewModel()
		{
		}

		private ICommand _loginCommand;

		public string UserName
		{
			get; set;
		}

		public string Password
		{
			get; set;
		}

		public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new Command(async () => await DoLoginCommandAsync()));

		private async Task DoLoginCommandAsync()
		{
			System.Diagnostics.Debug.WriteLine("User name : " + UserName + "\tPassword : " + Password);
			await Task.CompletedTask;
		}
	}
}
