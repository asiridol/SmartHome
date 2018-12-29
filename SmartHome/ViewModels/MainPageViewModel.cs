using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using SmartHome.Services.Cache;
using SmartHome.Services.KeyStore;
using SmartHome.Services.Network.Models;
using SmartHome.Services.Network.Mqtt;
using Xamarin.Forms;
using SmartHome.Services.SmartLight;
using System.ComponentModel;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SmartHome.ViewModels
{
	public class MainPageViewModel : ProgressAwareViewModel
	{
		private readonly Lazy<ISmartLightCache> _cache;
		private readonly Lazy<ISmartLightService> _smartLightService;

		private ICommand _initCommand;
		private ICommand _refreshRoomsCommand;
		private Command<RoomViewModel> _roomSelectedCommand;

		private ObservableCollection<RoomViewModel> _rooms = new ObservableCollection<RoomViewModel>();

		public MainPageViewModel(Lazy<ISmartLightCache> cache, Lazy<ISmartLightService> smartLightService)
		{
			_cache = cache;
			_smartLightService = smartLightService;

			MessagingCenter.Instance.Subscribe<IMqttService, MqttMessage>(this, MqttService.MqttMessageId, OnMessageReceived);
		}

		public event EventHandler ClearSelection;

		public ObservableCollection<RoomViewModel> Rooms => _rooms;

		public ICommand InitCommand => _initCommand ?? (_initCommand = new Command(async () => await DoInitAsync()));
		public ICommand RefreshRoomsCommand => _refreshRoomsCommand ?? (_refreshRoomsCommand = new Command(async () => await DoRefreshAsync()));
		public Command<RoomViewModel> RoomSelectedCommand => _roomSelectedCommand ?? (_roomSelectedCommand = new Command<RoomViewModel>(async (room) => await RoomSelectedAsync(room)));

		private async Task RoomSelectedAsync(RoomViewModel room)
		{
			if (room == null)
			{
				return;
			}

			if (room.IsOn == null)
			{
				return;
			}

			if (room.IsOn.GetValueOrDefault())
			{
				await _smartLightService.Value.TurnOffRoomAsync(room.RoomId);
			}
			else
			{
				await _smartLightService.Value.TurnOnRoomAsync(room.RoomId);
			}

			room.IsOn = !room.IsOn;

			ClearSelection?.Invoke(this, EventArgs.Empty);
		}

		private Task DoRefreshAsync()
		{
			return LoadRoomsAsync(true);
		}

		private async Task DoInitAsync()
		{
			await LoadRoomsAsync(false);
		}

		private async Task LoadRoomsAsync(bool forced)
		{
			try
			{
				IsBusy = true;

				if (forced)
				{
					await _cache.Value.ClearDeviceInfoAsync();
				}

				var cachedResponse = await _cache.Value.GetRoomsAsync(forced);
				Rooms.Clear();

				var defaultRoom = cachedResponse.Rooms.FirstOrDefault(x => x.RoomName == "DefaultRoom");
				var rooms = cachedResponse.Rooms.Except(new[] { defaultRoom }).Where(x => x.DeviceList.Any()).OrderByDescending(x => x.RoomOrder);

				foreach (var room in rooms)
				{
					var roomStatus = await _smartLightService.Value.GetRoomStatusAsync(room.RoomId);

					var roomVm = new RoomViewModel
					{
						RoomId = room.RoomId,
						RoomName = room.RoomName,
						IsOn = roomStatus,
						Devices = room.DeviceList
					};

					Rooms.Add(roomVm);
				}
			}
			finally
			{
				IsBusy = false;
			}
		}

		private void OnMessageReceived(IMqttService arg1, MqttMessage message)
		{
			if (message == null || string.IsNullOrEmpty(message.Payload))
			{
				return;
			}

			var deserialized = JsonConvert.DeserializeObject<List<DeviceStatus>>(message.Payload);
			var onOff = deserialized.FirstOrDefault(x => x.Type == "onOff");
			var onlineStatus = deserialized.FirstOrDefault(x => x.Type == "isOnline");
			var device = onOff.Dn;
			var room = Rooms.FirstOrDefault(x => x.Devices.Contains(device));

			if (onlineStatus != null && !onlineStatus.Value)
			{
				room.IsOn = null;
			}
			else
			{
				room.IsOn = onOff.Value;
			}
		}
	}
}
