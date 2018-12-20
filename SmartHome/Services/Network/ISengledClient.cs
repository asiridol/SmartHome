using System;
using System.Threading.Tasks;
using System.Threading;
using SmartHome.Services.Network.Models;

namespace SmartHome.Services.Network
{
	public interface ISengledClient
	{
		Task<string> AuthenticateClientAsync(string name, string password, string guid);
		Task<string> AuthenticateClientAsync(string name, string password, string guid, CancellationToken token);

		Task<bool> IsSessionTimeOutAsync(string sessionId, string guid);
		Task<bool> IsSessionTimeOutAsync(string sessionId, string guid, CancellationToken token);

		Task GetUserInfoAsync();
		Task GetUserInfoAsync(CancellationToken token);

		Task<RoomsResponse> GetRoomsInfoAsync(string sessionId);
		Task<RoomsResponse> GetRoomsInfoAsync(string sessionId, CancellationToken token);

		Task GetDevicesListAsync();
		Task GetDevicesListAsync(CancellationToken token);
	}
}
