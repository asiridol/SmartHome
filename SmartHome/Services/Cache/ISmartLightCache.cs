using System;
using System.Threading.Tasks;
using SmartHome.Services.Network.Models;
namespace SmartHome.Services.Cache
{
	public interface ISmartLightCache
	{
		Task<RoomsResponse> GetRoomsAsync(bool forced = false);
		Task<DevicesInfoResponse> GetDeviceInfoAsync();
	}
}
