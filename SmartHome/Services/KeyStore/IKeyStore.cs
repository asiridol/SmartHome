using System;
using System.Threading.Tasks;

namespace SmartHome.Services.KeyStore
{
	public interface IKeyStore
	{
		Task<bool> SaveKeyValueAsync<T>(KeyStoreKeys key, T value);
		Task<T> GetValueForKeyAsync<T>(KeyStoreKeys key);

		Task ClearStoreAsync();
	}
}
