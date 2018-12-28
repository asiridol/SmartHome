using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Connecting;

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
					_client.Connected += Connected;
					_client.Disconnected += Disconnected;
				}

				return _client;
			}
		}

		public async Task StartAsync(string jSessionToken, string userName, string hubId)
		{
			if (Client.IsConnected)
			{
				return;
			}

			var clientId = string.Format(ClientIdPattern, jSessionToken);
			var options = new MqttClientOptionsBuilder().WithClientId(clientId).WithWebSocketServer(WebSocketEndpoint).WithTls().WithCleanSession().WithKeepAlivePeriod(TimeSpan.FromHours(10)).Build();

			await Client.ConnectAsync(options);

			await Task.WhenAll(
				Client.SubscribeAsync($"element/{hubId}/status"),
				Client.SubscribeAsync($"ucenter/{userName}/action")
			);
		}

		private void Disconnected(object sender, MqttClientDisconnectedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Disconnected");
		}

		private void Connected(object sender, MqttClientConnectedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Connected");
		}

		private void MessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
		{
			var mqttMessage = new MqttMessage
			{
				Topic = e.ApplicationMessage.Topic,
				Payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload)
			};
		}

		public async Task<bool> SendMessageAsync(MqttMessage message)
		{
			var mqttAppmessage = new MqttApplicationMessageBuilder().WithTopic(message.Topic).WithPayload(message.Payload).WithExactlyOnceQoS().Build();
			var result = await Client.PublishAsync(mqttAppmessage);
			return result.ReasonCode == MQTTnet.Client.Publishing.MqttClientPublishReasonCode.Success;
		}
	}
}
