using System;
using Prism;
using Prism.Ioc;
using Prism.Autofac;
using Autofac;
using SmartHome.iOS.Services.Network;
using SmartHome.Services.Network;
using SmartHome.iOS.Services.KeyStore;
using SmartHome.Services.KeyStore;

namespace SmartHome.iOS
{
	public class IosPlatformInitializer : IPlatformInitializer
	{
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			var builder = containerRegistry.GetBuilder();
			builder.RegisterType<IosKeyStore>().As<IKeyStore>();
			builder.RegisterType<IosHttpClientFactory>().As<IHttpClientFactory>();
		}
	}
}
