using System;
using System.Threading;
using System.Threading.Tasks;
using SmartHome.Services.Network.Models;
using SmartHome.Services.Network.Base;
using System.Net.Http;
using System.Collections.Generic;

namespace SmartHome.Services.Network
{
	public class SengledClient : ISengledClient
	{
		private const string AuthenticateUrl = "https://ucenter.cloud.sengled.com/user/app/customer/v2/AuthenCross.json";
		private const string IsSessionTimeoutUrl = "https://ucenter.cloud.sengled.com/user/app/customer/isSessionTimeout.json";
		private const string DevicesListUrl = "https://elements.cloud.sengled.com/zigbee/device/getDeviceDetails.json";
		private const string RoomsListurl = "https://life2.cloud.sengled.com/life2/room/list.json";

		private const string CookieHeaderKey = "Cookie";
		private const string JSessionHeaderValue = "JSESSIONID={0}";

		private const string ProductAndAppId = "life";

		public Task<string> AuthenticateClientAsync(string name, string password, string guid)
		{
			return AuthenticateClientAsync(name, password, guid, CancellationToken.None);
		}

		public async Task<string> AuthenticateClientAsync(string name, string password, string guid, CancellationToken token)
		{
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

		public Task<bool> IsSessionTimeOutAsync(string sessionId, string guid)
		{
			return IsSessionTimeOutAsync(sessionId, guid, CancellationToken.None);
		}

		public async Task<bool> IsSessionTimeOutAsync(string sessionId, string guid, CancellationToken token)
		{
			var requestBody = new IsSessionTimeOutRequest
			{
				Uuid = guid,
				OsType = "android",
				AppCode = ProductAndAppId
			};

			var headers = GetHeaders(sessionId);
			var request = new JsonRequest<IsSessionTimeOutResponse>(headers) { Body = requestBody };
			var response = await request.SendRequestAsync(HttpMethod.Post, new Uri(IsSessionTimeoutUrl), token);
			return response.StatusCode == System.Net.HttpStatusCode.OK;
		}

		private Dictionary<string, string> GetHeaders(string sessionId)
		{
			return new Dictionary<string, string>
			{
				{
					CookieHeaderKey, string.Format(JSessionHeaderValue, sessionId)
				}
			};
		}

		public Task<DevicesInfoResponse> GetDeviceDetailsAsync(string sessionId)
		{
			return GetDeviceDetailsAsync(sessionId, CancellationToken.None);
		}

		public async Task<DevicesInfoResponse> GetDeviceDetailsAsync(string sessionId, CancellationToken token)
		{
			var headers = GetHeaders(sessionId);
			var request = new JsonRequest<DevicesInfoResponse>(headers);
			return await request.SendRequestAsync(HttpMethod.Post, new Uri(DevicesListUrl), token);
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

		public Task<RoomsResponse> GetRoomsInfoAsync(string sessionId)
		{
			return GetRoomsInfoAsync(sessionId, CancellationToken.None);
		}

		public async Task<RoomsResponse> GetRoomsInfoAsync(string sessionId, CancellationToken token)
		{
			var headers = GetHeaders(sessionId);
			var request = new JsonRequest<RoomsResponse>(headers);
			var response = await request.SendRequestAsync(HttpMethod.Post, new Uri(RoomsListurl));
			return response;
		}
	}
}
