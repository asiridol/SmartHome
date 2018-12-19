using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartHome.Services.KeyStore
{
	public abstract class AbstractKeyStore : IKeyStore
	{
		public async Task<T> GetValueForKeyAsync<T>(KeyStoreKeys key)
		{
			var stringValue = await GetStringValueForKey(key.ToString());

			if (string.IsNullOrEmpty(stringValue))
			{
				return default(T);
			}

			try
			{
				var deserialized = JsonConvert.DeserializeObject<T>(stringValue);
				return deserialized;
			}
			catch (JsonException)
			{
			}

			return default(T);
		}

		public async Task<bool> SaveKeyValueAsync<T>(KeyStoreKeys key, T value)
		{
			var stringValue = JsonConvert.SerializeObject(value);

			if (string.IsNullOrEmpty(stringValue))
			{
				return false;
			}
			try
			{
				var succes = await SaveKeyValueAsync(key.ToString(), stringValue);
				return succes;
			}
			catch (Exception)
			{
			}

			return false;
		}

		protected abstract Task<bool> SaveKeyValueAsync(string key, string value);
		protected abstract Task<string> GetStringValueForKey(string key);
		public abstract Task ClearStoreAsync();
	}
}
