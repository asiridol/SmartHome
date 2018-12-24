using System;
using System.Threading.Tasks;

namespace SmartHome.Services.Network.Mqtt
{
	public interface IMqttService
	{
		Task StartAsync(string jSessionToken, string hubId);
	}
}
