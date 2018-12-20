using System;
using System.Threading.Tasks;

namespace SmartHome.Services.SmartLight
{
	public interface ISmartLightService
	{
		Task TurnOnLightAsync(string groupId);
		Task TurnOffLightAsync(string groupId);
		Task<bool> GetLightStatusAsync(string groupId);
	}
}
