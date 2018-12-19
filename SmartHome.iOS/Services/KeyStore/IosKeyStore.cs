using System;
using System.Threading.Tasks;
using SmartHome.Services.KeyStore;
using Foundation;
using SmartHome.iOS.Extensions;
namespace SmartHome.iOS.Services.KeyStore
{
	public class IosKeyStore : AbstractKeyStore
	{
		public override Task ClearStoreAsync()
		{
			using (var nsUserDefaults = new NSUserDefaults())
			{
				nsUserDefaults.RemovePersistentDomain(NSBundle.MainBundle.BundleIdentifier);
			}

			return Task.CompletedTask;
		}

		protected override Task<string> GetStringValueForKey(string key)
		{
			var tcs = new TaskCompletionSource<string>();

			using (var nsUserDefaults = new NSUserDefaults())
			{
				var saved = nsUserDefaults.ValueForKey(key.ToNSString())?.ToString();
				tcs.TrySetResult(saved);
			}

			return tcs.Task;
		}

		protected override Task<bool> SaveKeyValueAsync(string key, string value)
		{
			var tcs = new TaskCompletionSource<bool>();

			try
			{
				using (var nsUserDefaults = new NSUserDefaults())
				{
					nsUserDefaults.SetValueForKey(key.ToNSString(), value.ToNSString());
					tcs.TrySetResult(true);
				}
			}
			catch (Exception)
			{
				tcs.TrySetResult(false);
			}

			return tcs.Task;
		}
	}
}
