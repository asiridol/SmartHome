using System;
using System.Threading.Tasks;
using System.Threading;
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

		Task GetDeviceDetailsAsync();
		Task GetDeviceDetailsAsync(CancellationToken token);

		Task GetDevicesListAsync();
		Task GetDevicesListAsync(CancellationToken token);
	}
}
