using System;
using Prism;
using Prism.Ioc;
using Prism.Autofac;
using Autofac;
using SmartHome.iOS.Services.Network;
using SmartHome.Services.Network;

namespace SmartHome.iOS
{
	public class IosPlatformInitializer : IPlatformInitializer
	{
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			var builder = containerRegistry.GetBuilder();
			builder.RegisterType<IosHttpClientFactory>().As<IHttpClientFactory>();
		}
	}
}
