using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SmartHome.Services.Network.Converters;

namespace SmartHome.Services.Network.Models
{
	public class DeviceStatus
	{
		[JsonProperty("dn")] public string Dn { get; set; }
		[JsonProperty("type")] public string Type { get; set; }
		[JsonProperty("value")] [JsonConverter(typeof(BooleanConverter))] public bool Value { get; set; }
		[JsonProperty("time")] public string Time { get; set; }
	}

	public enum Type
	{
		OnOff,
		IsOnline
	}
}
