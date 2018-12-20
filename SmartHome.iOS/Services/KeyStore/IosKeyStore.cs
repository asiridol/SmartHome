using System;
using System.Threading.Tasks;
using SmartHome.Services.KeyStore;
using Foundation;
using SmartHome.iOS.Extensions;
using System.Collections.Generic;
using UIKit;
namespace SmartHome.iOS.Services.KeyStore
{
	public class IosKeyStore : AbstractKeyStore
	{
		public override Task ClearStoreAsync()
		{
			return Task.Run(async () =>
			{
				var keys = Enum.GetValues(typeof(KeyStoreKeys));
				foreach (var key in keys)
				{
					var keystoreKey = (KeyStoreKeys)key;
					await SaveKeyValueAsync(keystoreKey, string.Empty);
				}
			});
		}

		protected override Task<string> GetStringValueForKey(string key)
		{
			return Task.Run(async () =>
			{
				if (UIApplication.SharedApplication.Delegate is AppDelegate appDelegate)
				{
					await appDelegate.EnsureInitializedAsync();
				}

				string saved = string.Empty;
				try
				{
					saved = NSUserDefaults.StandardUserDefaults.StringForKey(key);
				}
				catch (Exception)
				{
					saved = string.Empty;
				}

				return saved;
			});
		}

		protected override Task<bool> SaveKeyValueAsync(string key, string value)
		{
			return Task.Run(() =>
			{
				bool success = false;
				try
				{
					NSUserDefaults.StandardUserDefaults.SetString(value, key);
					success = true;
				}
				catch (Exception ex)
				{
					success = false;
				}

				return success;
			});
		}
	}
}
