using DryIoc;
using Prism;
using Prism.Ioc;
using SmartHome.iOS.Services.KeyStore;
using SmartHome.iOS.Services.Network;
using SmartHome.Services.KeyStore;
using SmartHome.Services.Network;
using SmartHome.Services.FileSystem;
using SmartHome.iOS.Services.FileSystem;

namespace SmartHome.iOS
{
	public class IosPlatformInitializer : IPlatformInitializer
	{
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.Register<IKeyStore, IosKeyStore>();
			containerRegistry.Register<IHttpClientFactory, IosHttpClientFactory>();
			containerRegistry.Register<IFilePaths, IosFilePaths>();
		}
	}
}
