using System;
using System.Threading;
using System.Threading.Tasks;
using SmartHome.Services.Network.Models;
using SmartHome.Services.Network.Base;
using System.Net.Http;

namespace SmartHome.Services.Network
{
	public class SengledClient : ISengledClient
	{
		private const string AuthenticateUrl = "https://ucenter.cloud.sengled.com/user/app/customer/v2/AuthenCross.json";
		private const string IsSessionTimeoutUrl = "https://ucenter.cloud.sengled.com/user/app/customer/isSessionTimeout.json";
		private const string DevicesListUrl = "https://life2.cloud.sengled.com/life2/device/list.json";

		private const string ProductAndAppId = "life";

		public Task<string> AuthenticateClientAsync(string name, string password)
		{
			return AuthenticateClientAsync(name, password, CancellationToken.None);
		}

		public async Task<string> AuthenticateClientAsync(string name, string password, CancellationToken token)
		{
			var guid = Guid.NewGuid().ToString("N").Substring(0, 16);
			var requestBody = new AuthenticationRequest
			{
				User = name,
				Passwod = password,
				OsType = "android",
				Uuid = guid,
				AppCode = ProductAndAppId,
				ProductCode = ProductAndAppId
			};

			var request = new JsonRequest<AuthenticateResponse>(null) { Body = requestBody };
			var response = await request.SendRequestAsync(HttpMethod.Post, new Uri(AuthenticateUrl), token);
			return response.JsonPropertysessionId;
		}

		public Task<bool> IsSessionTimeOutAsync()
		{
			return IsSessionTimeOutAsync(CancellationToken.None);
		}

		public async Task<bool> IsSessionTimeOutAsync(CancellationToken token)
		{
			var requestBody = new IsSessionTimeOutRequest
			{
				Uuid = Guid.NewGuid().ToString("N").Substring(0, 16),
				OsType = "android",
				AppCode = ProductAndAppId
			};

			var request = new JsonRequest<IsSessionTimeOutResponse>(null) { Body = requestBody };
			var response = await request.SendRequestAsync(HttpMethod.Post, new Uri(IsSessionTimeoutUrl), token);
			return response.StatusCode == System.Net.HttpStatusCode.OK;
		}

		public Task GetDeviceDetailsAsync()
		{
			return GetDeviceDetailsAsync(CancellationToken.None);
		}

		public Task GetDeviceDetailsAsync(CancellationToken token)
		{
			throw new NotImplementedException();
		}

		public Task GetDevicesListAsync()
		{
			return GetDevicesListAsync(CancellationToken.None);
		}

		public Task GetDevicesListAsync(CancellationToken token)
		{
			throw new NotImplementedException();
		}

		public Task GetUserInfoAsync()
		{
			return GetUserInfoAsync(CancellationToken.None);
		}

		public Task GetUserInfoAsync(CancellationToken token)
		{
			throw new NotImplementedException();
		}
	}
}
