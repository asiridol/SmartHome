using System;
namespace SmartHome.Services.Network.Mqtt
{
	public class MqttMessage
	{
		public string Topic { get; set; }
		public string Payload { get; set; }
	}
}
