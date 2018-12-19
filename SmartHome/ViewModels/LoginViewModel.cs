using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using SmartHome.Services.Network;
using SmartHome.Services.KeyStore;

namespace SmartHome.ViewModels
{
	public class LoginViewModel : ProgressAwareViewModel
	{
		private readonly Lazy<ISengledClient> _sengledClient;
		private readonly Lazy<IKeyStore> _keyStore;

		public LoginViewModel(Lazy<ISengledClient> sengledClient, Lazy<IKeyStore> keyStore)
		{
			_sengledClient = sengledClient;
			_keyStore = keyStore;
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

				var guid = Guid.NewGuid().ToString("N").Substring(0, 16);

				var sessionid = await _sengledClient.Value.AuthenticateClientAsync(UserName, Password, guid);

				if (!string.IsNullOrEmpty(sessionid))
				{
					var tasks = new[]{
						_keyStore.Value.SaveKeyValueAsync(KeyStoreKeys.JSessionId, sessionid),
						_keyStore.Value.SaveKeyValueAsync(KeyStoreKeys.UniqueDeviceId, guid)
					};
					var success = await Task.WhenAll(tasks);
				}
			}
			finally
			{
				IsBusy = false;
			}
		}
	}
}
