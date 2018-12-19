using System;
using System.Net.Http;
using SmartHome.Services.Network;
using Xamarin.Android.Net;

namespace SmartHome.Droid.Services.Network
{
	public class AndroidHttpClientFactory : AbstracttHttpClientFactory
	{
		protected override HttpMessageHandler GetClientHandler()
		{
			return new AndroidClientHandler() { UseCookies = false, CookieContainer = null };
		}
	}
}
