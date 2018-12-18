using System;
using System.Net.Http;
namespace SmartHome.Services.Network
{
	public interface IHttpClientFactory
	{
		HttpClient GetClient();
	}
}
