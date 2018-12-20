using System;
using System.Threading.Tasks;
using DryIoc;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using SmartHome.Services.KeyStore;
using SmartHome.Services.Network;
using SmartHome.ViewModels;
using SmartHome.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;
using Prism.Navigation;
using System.Linq;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SmartHome
{
	public partial class App : PrismApplication
	{
		private int _threadLock;

		public App() : this(null)
		{
		}

		public App(IPlatformInitializer initializer) : base(initializer)
		{
		}

		protected override async void OnInitialized()
		{
			InitializeComponent();

			// long running process will crash the app
			await NavigationService.NavigateAsync(nameof(LoadingPage));

			await NavigateToRootPageAsync();
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}

		protected override void RegisterTypes(IContainerRegistry containerRegistry)
		{
			var container = containerRegistry.GetContainer();

			//container.RegisterType<LoginPageViewModel>();
			container.Register<ISengledClient, SengledClient>();
			containerRegistry.RegisterForNavigation<LoadingPage>();
			containerRegistry.RegisterForNavigation<LoginPage>();
			containerRegistry.RegisterForNavigation<MainPage>();
			containerRegistry.Register<LoginPageViewModel>();
			containerRegistry.Register<MainPageViewModel>();
		}

		private async Task NavigateToRootPageAsync()
		{
			//if (Device.Idiom == TargetIdiom.Phone)
			//{
			//	await NavigationService.NavigateAsync(nameof(NavigationPage) + "/" + nameof(LoginPage));
			//}
			//else
			//{
			//	await NavigationService.NavigateAsync("/" + nameof(LoginPage));
			//}

			if (Interlocked.CompareExchange(ref _threadLock, 0, 1) != 0)
			{
				// already in progress
				return;
			}

			try
			{
				if (await IsLoggedInAsync())
				{
					await NavigationService.NavigateAsync("/" + nameof(Views.MainPage));
				}
				else
				{
					await NavigationService.NavigateAsync("/" + nameof(LoginPage));
				}
			}
			finally
			{

				Interlocked.Exchange(ref _threadLock, 0);
			}
		}

		private async Task<bool> IsLoggedInAsync()
		{
			//return false;
			var keyStore = Container.Resolve<IKeyStore>();

			var checkSessionTokenTask = Task.Run(async () =>
			{
				return !string.IsNullOrEmpty(await keyStore.GetValueForKeyAsync<string>(KeyStoreKeys.JSessionId));
			});

			var checkDeviceIdTask = Task.Run(async () =>
			{
				return !string.IsNullOrEmpty(await keyStore.GetValueForKeyAsync<string>(KeyStoreKeys.UniqueDeviceId));
			});

			return (await Task.WhenAll(checkSessionTokenTask, checkDeviceIdTask)).All(x => x);
		}
	}
}
