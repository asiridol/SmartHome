using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SmartHome.Services.Network.Models
{
	public class RoomsResponse
	{
		[JsonProperty("messageCode")] public string MessageCode { get; set; }
		[JsonProperty("info")] public string Info { get; set; }
		[JsonProperty("description")] public string Description { get; set; }
		[JsonProperty("roomList")] public List<RoomList> RoomList { get; set; }
		[JsonProperty("success")] public bool Success { get; set; }
	}

	public partial class RoomList
	{
		[JsonProperty("roomId")] public long RoomId { get; set; }
		[JsonProperty("roomName")] public string RoomName { get; set; }
		[JsonProperty("roomImgType")] public long RoomImgType { get; set; }
		[JsonProperty("roomImgUrl")] public string RoomImgUrl { get; set; }
		[JsonProperty("roomOrder")] public long RoomOrder { get; set; }
		[JsonProperty("attributeList")] public List<AttributeList> AttributeList { get; set; }
		[JsonProperty("deviceList")] public List<string> DeviceList { get; set; }
		[JsonProperty("scheduleList")] public List<long> ScheduleList { get; set; }
	}

	public partial class AttributeList
	{
		[JsonProperty("name")] public string Name { get; set; }
		[JsonProperty("value")] public string Value { get; set; }
	}
}
