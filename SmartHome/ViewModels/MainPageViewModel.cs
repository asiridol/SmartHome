using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using SmartHome.Services.SmartLight;
using SmartHome.Services.Cache;
using System.Collections;
using SmartHome.Services.Network.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SmartHome.Services.Network.Mqtt;
using SmartHome.Services.KeyStore;

namespace SmartHome.ViewModels
{
	public class MainPageViewModel : ProgressAwareViewModel
	{
		private readonly Lazy<ISmartLightCache> _cache;
		private readonly Lazy<IMqttService> _mqttClient;
		private readonly Lazy<IKeyStore> _keyStore;

		private ICommand _initCommand;
		private ICommand _refreshRoomsCommand;
		private Command<Room> _roomSelectedCommand;

		private string _hubId;
		private string _jToken;

		private ObservableCollection<Room> _rooms = new ObservableCollection<Room>();

		public MainPageViewModel(Lazy<ISmartLightCache> cache, Lazy<IMqttService> mqttClient, Lazy<IKeyStore> keyStore)
		{
			_cache = cache;
			_mqttClient = mqttClient;
			_keyStore = keyStore;
		}

		public ObservableCollection<Room> Rooms => _rooms;

		public ICommand InitCommand => _initCommand ?? (_initCommand = new Command(async () => await DoInitAsync()));
		public ICommand RefreshRoomsCommand => _refreshRoomsCommand ?? (_refreshRoomsCommand = new Command(async () => await DoRefreshAsync()));
		public Command<Room> RoomSelectedCommand => _roomSelectedCommand ?? (_roomSelectedCommand = new Command<Room>(async (room) => await RoomSelectedAsync(room)));

		private Task RoomSelectedAsync(Room room)
		{
			System.Diagnostics.Debug.WriteLine("Room: " + room.RoomId);

			return Task.CompletedTask;
		}

		private Task DoRefreshAsync()
		{
			return LoadRoomsAsync(true);
		}

		private async Task DoInitAsync()
		{
			await Task.WhenAll(LoadRoomsAsync(false), LoadHubIdAsync());
		}

		private async Task LoadHubIdAsync()
		{
			_jToken = await _keyStore.Value.GetValueForKeyAsync<string>(KeyStoreKeys.JSessionId);
			var devicesResponse = await _cache.Value.GetDeviceInfoAsync();

			if (devicesResponse != null)
			{
				_hubId = devicesResponse.DeviceInfos.FirstOrDefault().GatewayUuid;
			}

			await _mqttClient.Value.StartAsync(_jToken, _hubId);
		}

		private async Task LoadRoomsAsync(bool forced)
		{
			try
			{
				IsBusy = true;
				var cachedResponse = await _cache.Value.GetRoomsAsync(forced);
				Rooms.Clear();

				var rooms = cachedResponse.Rooms.Distinct().Where(x => x.DeviceList.Any()).OrderByDescending(x => x.RoomOrder);

				foreach (var room in rooms)
				{
					Rooms.Add(room);
				}
			}
			finally
			{

				IsBusy = false;
			}
		}
	}
}
