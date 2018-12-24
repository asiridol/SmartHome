using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartHome.Services.Network.Models
{
	public class RoomsResponse
	{
		[JsonProperty("messageCode")] public string MessageCode { get; set; }
		[JsonProperty("info")] public string Info { get; set; }
		[JsonProperty("description")] public string Description { get; set; }
		[JsonProperty("roomList")] public List<Room> Rooms { get; set; }
		[JsonProperty("success")] public bool Success { get; set; }
	}

	public partial class Room : IEquatable<Room>
	{
		[JsonProperty("roomId")] public long RoomId { get; set; }
		[JsonProperty("roomName")] public string RoomName { get; set; }
		[JsonProperty("roomImgType")] public long RoomImgType { get; set; }
		[JsonProperty("roomImgUrl")] public string RoomImgUrl { get; set; }
		[JsonProperty("roomOrder")] public long RoomOrder { get; set; }
		[JsonProperty("attributeList")] public List<AttributeList> AttributeList { get; set; }
		[JsonProperty("deviceList")] public List<string> DeviceList { get; set; }
		[JsonProperty("scheduleList")] public List<long> ScheduleList { get; set; }

		public bool Equals(Room x, Room y)
		{
			if (x == null || y == null)
			{
				return false;
			}

			return x.DeviceList.Equals(y.DeviceList);
		}

		public bool Equals(Room other)
		{
			return Equals(this, other);
		}

		public int GetHashCode(Room obj)
		{
			var hashCode = 0x00001;
			if (obj == null)
			{
				return hashCode;
			}

			foreach (var device in obj.DeviceList)
			{
				hashCode += device.GetHashCode();
			}

			return hashCode;
		}
	}

	public partial class AttributeList
	{
		[JsonProperty("name")] public string Name { get; set; }
		[JsonProperty("value")] public string Value { get; set; }
	}
}
