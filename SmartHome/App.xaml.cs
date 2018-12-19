using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Prism.Autofac;
using SmartHome.Views;
using Prism.Ioc;
using SmartHome.ViewModels;
using System.Threading.Tasks;
using Prism;
using Autofac;
using SmartHome.Services.Network;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace SmartHome
{
	public partial class App : PrismApplication
	{
		public App() : this(null)
		{
		}

		public App(IPlatformInitializer initializer) : base(initializer)
		{
			//MainPage = new NavigationPage(new LoginPage());
		}

		protected override async void OnInitialized()
		{
			InitializeComponent();

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
			var builder = containerRegistry.GetBuilder();

			builder.RegisterType<LoginViewModel>();
			builder.RegisterType<SengledClient>().As<ISengledClient>();
			containerRegistry.RegisterForNavigation<ContentPage>();
			containerRegistry.RegisterForNavigation<LoginPage>();
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

			await NavigationService.NavigateAsync("/" + nameof(LoginPage));
		}
	}
}
