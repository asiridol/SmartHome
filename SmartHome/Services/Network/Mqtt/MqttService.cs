using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;

namespace SmartHome.Services.Network.Mqtt
{
	public class MqttService : IMqttService
	{
		private const string WebSocketEndpoint = "wss://ap-mqtt.cloud.sengled.com:443/mqtt";
		private const string ClientIdPattern = "{0}@elementapp";

		private IMqttClient _client;

		private IMqttClient Client
		{
			get
			{
				if (_client == null)
				{
					var factory = new MqttFactory();
					_client = factory.CreateMqttClient();

					_client.ApplicationMessageReceived += MessageReceived;
					_client.Disconnected += Disconnected;
				}

				return _client;
			}
		}

		public async Task StartAsync(string jSessionToken, string hubId)// string userName, string hubId)
		{
			if (Client.IsConnected)
			{
				return;
			}

			var clientId = string.Format(ClientIdPattern, jSessionToken);
			var options = new MqttClientOptionsBuilder().WithClientId(clientId).WithWebSocketServer(WebSocketEndpoint).WithTls().WithCleanSession().Build();

			await Client.ConnectAsync(options);

			await Client.SubscribeAsync($"element/{hubId}/status");
			//await Client.SubscribeAsync($"ucenter/{userName}/action");
		}

		private void Disconnected(object sender, MqttClientDisconnectedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Disconnected");
		}

		private double ToUnixTime(DateTime date)
		{
			var epoch = Math.Round((date - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds, 0);
			return epoch;
		}

		private void MessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
			System.Diagnostics.Debug.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
			System.Diagnostics.Debug.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
			System.Diagnostics.Debug.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
			System.Diagnostics.Debug.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
		}
	}
}
