using System;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Connecting;
using Xamarin.Forms;

namespace SmartHome.Services.Network.Mqtt
{
	public class MqttService : IMqttService
	{
		private const string WebSocketEndpoint = "wss://ap-mqtt.cloud.sengled.com:443/mqtt";
		private const string ClientIdPattern = "{0}@elementapp";

		public static string MqttMessageId = "mqtt.message";

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

		public async Task StartAsync(string jSessionToken, string hubId, string userName)
		{
			if (Client.IsConnected)
			{
				return;
			}

			var clientId = string.Format(ClientIdPattern, jSessionToken);
			var options = new MqttClientOptionsBuilder().WithClientId(clientId).WithWebSocketServer(WebSocketEndpoint).WithTls().WithCleanSession().Build();

			await Client.ConnectAsync(options);

			var hubTopic = $"element/{hubId}/status";
			await Client.SubscribeAsync(hubTopic);
			var userTopic = $"ucenter/{userName}/action";
			await Client.SubscribeAsync(userTopic);
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
			System.Diagnostics.Debug.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
			System.Diagnostics.Debug.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
			System.Diagnostics.Debug.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
			System.Diagnostics.Debug.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
			System.Diagnostics.Debug.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");

			var mqttMessage = new MqttMessage
			{
				Topic = e.ApplicationMessage.Topic,
				Payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload)
			};

			MessagingCenter.Instance.Send<IMqttService, MqttMessage>(this, MqttMessageId, mqttMessage);
		}

		public async Task<bool> SendMessageAsync(MqttMessage message)
		{
			var mqttAppmessage = new MqttApplicationMessageBuilder().WithTopic(message.Topic).WithPayload(message.Payload).WithExactlyOnceQoS().Build();
			var result = await Client.PublishAsync(mqttAppmessage);
			return result.ReasonCode == MQTTnet.Client.Publishing.MqttClientPublishReasonCode.Success;
		}
	}
}
