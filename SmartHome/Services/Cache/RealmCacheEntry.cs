using System;
using Akavache;
using Realms;
using Realms.Weaving;
using Newtonsoft.Json;
namespace SmartHome.Services.Cache
{
	public class RealmCacheEntry : RealmObject
	{
		public string SerializedCacheEntry { get; set; }

		[Ignored]
		public CacheEntry Entry
		{
			get
			{
				if (string.IsNullOrEmpty(SerializedCacheEntry))
				{
					return null;
				}

				return JsonConvert.DeserializeObject<CacheEntry>(SerializedCacheEntry);
			}
			set
			{
				if (value == null)
				{
					SerializedCacheEntry = string.Empty;
					return;
				}
				SerializedCacheEntry = JsonConvert.SerializeObject(value);
			}
		}

		[PrimaryKey]
		public string Key { get; set; }
	}
}
