using System;
using System.Threading.Tasks;

namespace SmartHome.Services.SmartLight
{
	public interface ISmartLightService
	{
		Task TurnOnRoomAsync(long groupId);
		Task TurnOffRoomAsync(long groupId);
		Task<bool?> GetRoomStatusAsync(long groupId);
	}
}
