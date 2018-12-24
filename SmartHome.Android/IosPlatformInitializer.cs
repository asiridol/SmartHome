using System;
using Prism;
using Prism.Ioc;
using SmartHome.Droid.Services.Network;
using SmartHome.Services.Network;

namespace SmartHome.iOS
{
	public class AndroidPlatformInitializer : IPlatformInitializer
	{
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			var builder = containerRegistry.GetBuilder();
			builder.RegisterType<AndroidHttpClientFactory>().As<IHttpClientFactory>();
		}
	}
}
