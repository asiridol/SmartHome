using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using SmartHome.Services.Network;
using SmartHome.Services.KeyStore;
using Prism.Navigation;
using SmartHome.Views;
using Prism.Ioc;
using System.Linq;
using Prism.Services;

namespace SmartHome.ViewModels
{
	public class LoginPageViewModel : ProgressAwareViewModel
	{
		private readonly Lazy<ISengledClient> _sengledClient;
		private readonly Lazy<IKeyStore> _keyStore;
		private readonly Lazy<INavigationService> _navigation;
		private readonly Lazy<IPageDialogService> _dialogService;

		private ICommand _loginCommand;

		public LoginPageViewModel(Lazy<ISengledClient> sengledClient, Lazy<IKeyStore> keyStore, Lazy<INavigationService> navigation, Lazy<IPageDialogService> dialogService)
		{
			_sengledClient = sengledClient;
			_keyStore = keyStore;
			_navigation = navigation;
			_dialogService = dialogService;
		}

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
			bool success = false;
			try
			{
				IsBusy = true;

				var guid = Guid.NewGuid().ToString("N").Substring(0, 16);

				var sessionid = await _sengledClient.Value.AuthenticateClientAsync(UserName, Password, guid);

				if (!string.IsNullOrEmpty(sessionid))
				{
					var tasks = new[]{
						_keyStore.Value.SaveKeyValueAsync(KeyStoreKeys.JSessionId, sessionid),
						_keyStore.Value.SaveKeyValueAsync(KeyStoreKeys.UniqueDeviceId, guid)
					};

					success = (await Task.WhenAll(tasks)).All(x => x);
				}
			}
			finally
			{
				IsBusy = false;
				if (success)
				{
					await _navigation.Value.NavigateAsync("/" + nameof(MainPage));
				}
				else
				{
					await _dialogService.Value.DisplayAlertAsync("Login error", "Check username and password", "OK");
				}
			}
		}
	}
}
