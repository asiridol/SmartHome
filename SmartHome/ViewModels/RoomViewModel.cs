using System;
using System.Collections.Generic;
using SmartHome.ViewModels;
using Xamarin.Forms;

namespace SmartHome.ViewModels
{
	public class RoomViewModel : ViewModelBase
	{
		private bool? _isOn;

		public string RoomName { get; set; }
		public long RoomId { get; set; }
		public bool? IsOn
		{
			get => _isOn;
			set
			{
				Set(() => IsOn, ref _isOn, value);
			}
		}
		public List<string> Devices { get; set; }
	}
}
