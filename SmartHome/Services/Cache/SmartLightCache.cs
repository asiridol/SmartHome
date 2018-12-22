using System;
using Akavache;
using SmartHome.Services.Network;
using SmartHome.Services.Network.Models;
using SmartHome.Services.KeyStore;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace SmartHome.Services.Cache
{
	public class SmartLightCache : ISmartLightCache
	{
		private const string RoomsCacheKey = "Rooms";
		private const string DeviceInfoCacheKey = "DeviceInfo";

		private readonly Lazy<ISengledClient> _sengledClient;
		private readonly Lazy<IKeyStore> _keyStore;
		private readonly TimeSpan _expiration = TimeSpan.FromHours(2);

		public SmartLightCache(Lazy<IObjectBlobCache> cacheStore, Lazy<ISengledClient> sengledClient, Lazy<IKeyStore> keyStore)
		{
			BlobCache.ApplicationName = "SmartLights";
			BlobCache.ForcedDateTimeKind = DateTimeKind.Local;
			BlobCache.LocalMachine = cacheStore.Value;

			_sengledClient = sengledClient;
			_keyStore = keyStore;
		}

		public async Task<RoomsResponse> GetRoomsAsync(bool forced = false)
		{
			if (forced)
			{
				await BlobCache.LocalMachine.Invalidate(RoomsCacheKey);
			}

			var sessionId = await _keyStore.Value.GetValueForKeyAsync<string>(KeyStoreKeys.JSessionId);
			return await BlobCache.LocalMachine.GetOrFetchObject(RoomsCacheKey, async () => await _sengledClient.Value.GetRoomsInfoAsync(sessionId), DateTimeOffset.UtcNow.Add(_expiration));
		}

		public async Task<DevicesInfoResponse> GetDeviceInfoAsync()
		{
			var sessionId = await _keyStore.Value.GetValueForKeyAsync<string>(KeyStoreKeys.JSessionId);
			return await BlobCache.LocalMachine.GetOrFetchObject(DeviceInfoCacheKey, async () => await _sengledClient.Value.GetDeviceDetailsAsync(sessionId), DateTimeOffset.UtcNow.Add(_expiration));
		}
	}
}
