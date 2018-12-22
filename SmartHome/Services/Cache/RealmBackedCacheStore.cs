using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akavache;
using Newtonsoft.Json;
using Realms;
using System.Linq;
using Newtonsoft.Json.Bson;
using SmartHome.Services.FileSystem;

namespace SmartHome.Services.Cache
{
	public class RealmBackedCacheStore : IObjectBlobCache, IDisposable
	{
		private readonly SemaphoreSlim _initLock = new SemaphoreSlim(1);
		private readonly Lazy<IFilePaths> _filePaths;
		private bool _isDisposed;

		public RealmBackedCacheStore(Lazy<IFilePaths> filePaths)
		{
			Scheduler = CurrentThreadScheduler.Instance;
			_filePaths = filePaths;
		}

		public IScheduler Scheduler { get; private set; }

		public void Dispose()
		{
			Scheduler = null;
			_isDisposed = true;
		}

		public IObservable<Unit> Shutdown
		{
			get
			{
				return Observable.StartAsync(async observer =>
				{
					await Task.CompletedTask;
				});
			}
		}

		private async Task<Realm> GetRealmDatabaseAsync()
		{
			var configs = new RealmConfiguration("realm.db");
			var realm = await Realm.GetInstanceAsync();
			await realm.RefreshAsync();
			return realm;
		}

		public IObservable<Unit> Flush()
		{
			return Observable.StartAsync(
									 observer =>
									 {
										 CheckDisposed();
										 return Task.FromResult(Unit.Default);
									 });
		}

		public IObservable<Unit> Insert(string key, byte[] data, DateTimeOffset? absoluteExpiration = default(DateTimeOffset?))
		{
			return Observable.StartAsync(
										async () =>
										{
											if (_isDisposed)
											{
												throw new ObjectDisposedException("CacheStore");
											}

											var cacheEntry = new CacheEntry(null, data, Scheduler.Now, absoluteExpiration);

											using (var realm = await GetRealmDatabaseAsync())
											{
												await realm.WriteAsync(x => x.Add(new RealmCacheEntry { Entry = cacheEntry, Key = key }));
											}
										});
		}

		public IObservable<byte[]> Get(string key)
		{
			return Observable.StartAsync(
										async () =>
										{
											CheckDisposed();

											using (var realm = await GetRealmDatabaseAsync())
											{
												var saved = realm.All<RealmCacheEntry>().Where(x => x.Key == key);

												if (!saved.Any())
												{
													throw new KeyNotFoundException($"Key : {key}");
												}

												var savedEntry = saved.FirstOrDefault();

												var entry = savedEntry.Entry;
												if (entry.ExpiresAt != null && Scheduler.Now > entry.ExpiresAt.Value)
												{
													using (var transaction = realm.BeginWrite())
													{
														realm.Remove(savedEntry);
														transaction.Commit();
													}
													throw new KeyNotFoundException($"Key : {key}");
												}

												return entry.Value;
											}
										});
		}

		public IObservable<IEnumerable<string>> GetAllKeys()
		{
			return Observable.StartAsync(
										async () =>
										{
											CheckDisposed();

											using (var realm = await GetRealmDatabaseAsync())
											{
												var entries = realm.All<RealmCacheEntry>().Where(x => x.Entry.ExpiresAt == null || x.Entry.ExpiresAt >= Scheduler.Now).Select(x => x.Key);
												return entries;
											}
										});
		}

		public IObservable<DateTimeOffset?> GetCreatedAt(string key)
		{
			return Observable.StartAsync<DateTimeOffset?>(
										async () =>
										{
											CheckDisposed();
											using (var realm = await GetRealmDatabaseAsync())
											{
												var savedEntry = realm.All<RealmCacheEntry>().FirstOrDefault(x => x.Key == key);
												if (savedEntry == null)
												{
													throw new KeyNotFoundException($"Key : {key}");
												}

												var entry = savedEntry.Entry;
												return entry.CreatedAt;
											}
										});
		}

		public IObservable<Unit> Invalidate(string key)
		{
			return Observable.StartAsync(
										async () =>
										{
											CheckDisposed();
											using (var realm = await GetRealmDatabaseAsync())
											{
												var entry = realm.All<RealmCacheEntry>().FirstOrDefault(x => x.Key == key);
												using (var transaction = realm.BeginWrite())
												{
													realm.Remove(entry);
													transaction.Commit();
												}
											}
										});
		}

		public IObservable<Unit> InvalidateAll()
		{
			return Observable.StartAsync(
										async () =>
										{
											CheckDisposed();
											using (var realm = await GetRealmDatabaseAsync())
											{
												using (var transaction = realm.BeginWrite())
												{
													realm.RemoveAll<RealmCacheEntry>();
													transaction.Commit();
												}
											}
										});
		}

		public IObservable<Unit> Vacuum()
		{
			return Observable.StartAsync(
										async () =>
										{
											CheckDisposed();
											using (var realm = await GetRealmDatabaseAsync())
											{
												var toDelete = realm.All<RealmCacheEntry>().Where(x => x.Entry.ExpiresAt >= Scheduler.Now);
												using (var transaction = realm.BeginWrite())
												{
													realm.RemoveRange<RealmCacheEntry>(toDelete);
													transaction.Commit();
												}
											}
										});
		}

		public IObservable<Unit> InsertObject<T>(string key, T value, DateTimeOffset? absoluteExpiration = default(DateTimeOffset?))
		{
			return Observable.StartAsync(
										async () =>
										{
											CheckDisposed();
											var data = SerializeObject(value);
											await Insert(key, data, absoluteExpiration);
										});
		}

		public IObservable<T> GetObject<T>(string key)
		{
			return Observable.StartAsync(async () =>
			{
				var saved = await Get(key);
				var deserialized = DeserializeObject<T>(saved);
				return deserialized;
			});
		}

		public IObservable<IEnumerable<T>> GetAllObjects<T>()
		{
			return Observable.StartAsync(
										async () =>
										{
											CheckDisposed();
											using (var realm = await GetRealmDatabaseAsync())
											{
												return realm.All<RealmCacheEntry>().Where(
															 x => x.Entry.TypeName == typeof(T).FullName &&
																  (x.Entry.ExpiresAt == null ||
																   x.Entry.ExpiresAt >= Scheduler.Now))
													  .Select(x => DeserializeObject<T>(x.Entry.Value));
											}
										});
		}

		public IObservable<DateTimeOffset?> GetObjectCreatedAt<T>(string key)
		{
			return GetCreatedAt(key);
		}

		public IObservable<Unit> InvalidateObject<T>(string key)
		{
			CheckDisposed();
			return Invalidate(key);
		}

		public IObservable<Unit> InvalidateAllObjects<T>()
		{
			return Observable.StartAsync(
										async () =>
										{
											CheckDisposed();

											using (var realm = await GetRealmDatabaseAsync())
											{
												var toDelete = realm.All<RealmCacheEntry>().Where(x => x.Entry.TypeName == typeof(T).FullName);

												await realm.WriteAsync(x => realm.RemoveRange<RealmCacheEntry>(toDelete));
											}
										});
		}

		private void CheckDisposed()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException("CacheStore");
			}
		}

		private byte[] SerializeObject<T>(T value)
		{
			var settings = new JsonSerializerSettings();
			var ms = new MemoryStream();
			var serializer = JsonSerializer.Create(settings);
			var writer = new BsonDataWriter(ms);

			serializer.Serialize(writer, new ObjectWrapper<T>() { Value = value });
			return ms.ToArray();
		}

		private T DeserializeObject<T>(byte[] data)
		{
			var settings = new JsonSerializerSettings();
			var serializer = JsonSerializer.Create(settings);
			var reader = new BsonDataReader(new MemoryStream(data));
			var forcedDateTimeKind = BlobCache.ForcedDateTimeKind;

			if (forcedDateTimeKind.HasValue)
			{
				reader.DateTimeKindHandling = forcedDateTimeKind.Value;
			}

			try
			{
				return serializer.Deserialize<ObjectWrapper<T>>(reader).Value;
			}
			catch (Exception)
			{
				Debug.WriteLine("Failed to deserialize data as boxed, we may be migrating from an old Akavache");
			}

			return serializer.Deserialize<T>(reader);
		}

		private class ObjectWrapper<T>
		{
			public T Value
			{
				get; set;
			}
		}
	}
}
