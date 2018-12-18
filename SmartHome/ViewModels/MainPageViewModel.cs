using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using SmartHome.Services.Network;

namespace SmartHome.ViewModels
{
	public class MainPageViewModel : ViewModelBase
	{
		private readonly Lazy<ISengledClient> _sengledClient;
		private bool _progress;

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

		public bool Progress
		{
			get
			{
				return _progress;
			}
			set
			{
				if (_progress == value)
				{
					return;
				}

				_progress = value;
				RaisePropertyChanged(nameof(Progress));
			}
		}

		public ICommand LoginCommand => _loginCommand ?? (_loginCommand = new Command(async () => await DoLoginCommandAsync()));

		private async Task DoLoginCommandAsync()
		{
			try
			{
				Progress = true;
				await Task.Delay(TimeSpan.FromSeconds(10));
				//await _sengledClient.Value.AuthenticateClientAsync(UserName, Password);
			}
			finally
			{
				Progress = false;
			}
		}
	}
}
