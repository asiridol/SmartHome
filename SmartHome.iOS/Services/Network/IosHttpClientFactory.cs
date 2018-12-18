using System;
using System.Net.Http;
using SmartHome.Services.Network;

namespace SmartHome.iOS.Services.Network
{
	public class IosHttpClientFactory : AbstracttHttpClientFactory
	{
		protected override HttpMessageHandler GetClientHandler()
		{
			return new NSUrlSessionHandler();
		}
	}
}
