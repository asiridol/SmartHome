using System;
using System.Threading.Tasks;
using System.Threading;
namespace SmartHome.Services.Network
{
	public interface ISengledClient
	{
		Task<string> AuthenticateClientAsync(string name, string password);
		Task<string> AuthenticateClientAsync(string name, string password, CancellationToken token);

		Task<bool> IsSessionTimeOutAsync();
		Task<bool> IsSessionTimeOutAsync(CancellationToken token);

		Task GetUserInfoAsync();
		Task GetUserInfoAsync(CancellationToken token);

		Task GetDeviceDetailsAsync();
		Task GetDeviceDetailsAsync(CancellationToken token);

		Task GetDevicesListAsync();
		Task GetDevicesListAsync(CancellationToken token);
	}
}
