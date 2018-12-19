using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using SmartHome.Services.Network;

namespace SmartHome.ViewModels
{
	public class MainPageViewModel : ProgressAwareViewModel
	{
		private readonly Lazy<ISengledClient> _sengledClient;

		public MainPageViewModel(Lazy<ISengledClient> sengledClient)
		{
			_sengledClient = sengledClient;
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
			try
			{
				IsBusy = true;
				await Task.Delay(TimeSpan.FromSeconds(10));
				//await _sengledClient.Value.AuthenticateClientAsync(UserName, Password);
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}
