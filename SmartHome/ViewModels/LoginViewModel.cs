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
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace SmartHome.ViewModels
{
	public class LoginPageViewModel : ProgressAwareViewModel
	{
		private readonly Lazy<ISengledClient> _sengledClient;
		private readonly Lazy<IKeyStore> _keyStore;
		private readonly Lazy<INavigationService> _navigation;
		private readonly Lazy<IPageDialogService> _dialogService;

		private static readonly Lazy<Regex> EmailRegex = new Lazy<Regex>(() => new Regex("^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z_]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z_])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase));

		private ValidatableProperty<string> _userName = new ValidatableProperty<string>();
		private ValidatableProperty<string> _password = new ValidatableProperty<string>();

		private ICommand _loginCommand;

		public LoginPageViewModel(Lazy<ISengledClient> sengledClient, Lazy<IKeyStore> keyStore, Lazy<INavigationService> navigation, Lazy<IPageDialogService> dialogService)
		{
			_sengledClient = sengledClient;
			_keyStore = keyStore;
			_navigation = navigation;
			_dialogService = dialogService;

			UserName.AddRule(x => !string.IsNullOrEmpty(x), "Email address cannot be empty");
			UserName.AddRule(x => EmailRegex.Value.IsMatch(x), "Email address is not valid");
			Password.AddRule(x => !string.IsNullOrEmpty(x), "Password cannot be empty");
			Password.AddRule(x => x.Length > 4, "Password has to be atleast 4 characters long");
		}

		private void ValidatablePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RaisePropertyChanged(nameof(sender));
		}

		public ValidatableProperty<string> UserName
		{
			get => _userName;
		}

		public ValidatableProperty<string> Password
		{
			get => _password;
		}

		public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new Command(async () => await DoLoginCommandAsync()));

		private async Task DoLoginCommandAsync()
		{
			bool success = false;

			Password.Validate();
			UserName.Validate();

			if (!UserName.IsValid || !Password.IsValid)
			{
				return;
			}

			try
			{
				IsBusy = true;

				var guid = Guid.NewGuid().ToString("N").Substring(0, 16);

				var sessionid = await _sengledClient.Value.AuthenticateClientAsync(UserName.Property, Password.Property, guid);

				if (!string.IsNullOrEmpty(sessionid))
				{
					var tasks = new[]{
						_keyStore.Value.SaveKeyValueAsync(KeyStoreKeys.JSessionId, sessionid),
						_keyStore.Value.SaveKeyValueAsync(KeyStoreKeys.UniqueDeviceId, guid),
						_keyStore.Value.SaveKeyValueAsync(KeyStoreKeys.Username, UserName)
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
