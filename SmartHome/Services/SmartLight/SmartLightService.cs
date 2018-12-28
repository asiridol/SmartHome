using System;
using SmartHome.Services.Cache;
using SmartHome.Services.KeyStore;
using SmartHome.Services.Network.Mqtt;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using SmartHome.Services.Network.Models;
using SmartHome.Services.Network;

namespace SmartHome.Services.SmartLight
{
	public class SmartLightService : ISmartLightService
	{
		private readonly Lazy<ISmartLightCache> _cache;
		private readonly Lazy<IMqttService> _mqttService;
		private readonly Lazy<IKeyStore> _keyStore;
		private readonly Lazy<ISengledClient> _sengledClient;

		private SemaphoreSlim _initLock = new SemaphoreSlim(1);
		private string _hubId;

		private DevicesInfoResponse _deviceInfoResponse;

		public SmartLightService(Lazy<ISmartLightCache> cache, Lazy<IMqttService> mqttService, Lazy<IKeyStore> keyStore, Lazy<ISengledClient> sengledClient)
		{
			_cache = cache;
			_mqttService = mqttService;
			_keyStore = keyStore;
			_sengledClient = sengledClient;

			_initLock.Wait();
			Task.Run(InitAsync);
		}

		private async Task InitAsync()
		{
			try
			{
				var tasks = new Task<string>[]
				{
					 _keyStore.Value.GetValueForKeyAsync<string>(KeyStoreKeys.JSessionId),
					_keyStore.Value.GetValueForKeyAsync<string>(KeyStoreKeys.Username),
					_keyStore.Value.GetValueForKeyAsync<string>(KeyStoreKeys.UniqueDeviceId)
				};

				await Task.WhenAll(tasks);

				var jToken = await tasks[0];
				var userName = await tasks[1];
				var uUid = await tasks[2];

				//check if session has expired
				var isNotExpired = await _sengledClient.Value.IsSessionTimeOutAsync(jToken, uUid);
				if (!isNotExpired)
				{
					System.Diagnostics.Debug.WriteLine("Session has expired");
				}

				await _cache.Value.ClearDeviceInfoAsync();

				_deviceInfoResponse = await _cache.Value.GetDeviceInfoAsync();

				if (_deviceInfoResponse != null)
				{
					_hubId = _deviceInfoResponse.DeviceInfos.FirstOrDefault().GatewayUuid;
				}

				await _mqttService.Value.StartAsync(jToken, _hubId, userName);
			}
			catch (Exception ex)
			{
			}
			finally
			{
				_initLock.Release();
			}
		}

		private void EnsureInitialized()
		{
			_initLock.Wait();
			_initLock.Release();
		}

		public async Task TurnOnRoomAsync(long groupId)
		{
			await _mqttService.Value.SendMessageAsync(await GetMessageAsync(groupId, true));
		}

		public async Task TurnOffRoomAsync(long groupId)
		{
			await _mqttService.Value.SendMessageAsync(await GetMessageAsync(groupId, false));
		}

		public async Task<bool?> GetRoomStatusAsync(long groupId)
		{
			EnsureInitialized();
			var devices = (await _cache.Value.GetRoomsAsync()).Rooms.FirstOrDefault(x => x.RoomId == groupId).DeviceList;
			var lamps = _deviceInfoResponse.DeviceInfos.FirstOrDefault().Lamps?.Where(lamp => devices.Contains(lamp.LampId));
			var onlineStatuses = lamps?.Select(x => x.Attributes.IsOnline);

			// atleast one bulb is offline
			if (onlineStatuses.Any(x => !x))
			{
				return null;
			}

			return lamps.All(x => x.Attributes.OnOff);
		}

		private async Task<MqttMessage> GetMessageAsync(long groupId, bool turnOn)
		{
			EnsureInitialized();

			var cachedRooms = await _cache.Value.GetRoomsAsync();
			var room = cachedRooms.Rooms.FirstOrDefault(x => x.RoomId == groupId);
			var devices = room.DeviceList;

			string serializedDevices = string.Empty;

			var request = new LightTriggerRequest
			{
				HubId = _hubId,
				TimeStamp = ToUnixTime(DateTime.UtcNow),
				Value = new PropertyValue
				{
					LampsList = devices,
					Switch = turnOn ? 1 : 0
				}
			};

			var serializedRequest = JsonConvert.SerializeObject(request, Formatting.None);
			var messageTopic = $"element/{_hubId}/update";
			var messagePayload = serializedRequest;

			return new MqttMessage
			{
				Topic = messageTopic,
				Payload = messagePayload
			};
		}

		private double ToUnixTime(DateTime date)
		{
			var epoch = Math.Round((date - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds, 0);
			return epoch;
		}
	}
}
