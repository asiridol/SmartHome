using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel;
using SmartHome.Services.Network.Converters;

namespace SmartHome.Services.Network.Models
{
	public class DevicesInfoResponse
	{
		[JsonProperty("deviceInfos")] public List<DeviceInfo> DeviceInfos { get; set; }
	}

	public partial class DeviceInfo
	{
		[JsonProperty("gatewayUuid")] public string GatewayUuid { get; set; }
		[JsonProperty("gatewayName")] public string GatewayName { get; set; }
		[JsonProperty("routerBssid")] public string RouterBssid { get; set; }
		[JsonProperty("activeTime")] public DateTimeOffset ActiveTime { get; set; }
		[JsonProperty("gatewayVersion")] public string GatewayVersion { get; set; }
		[JsonProperty("gatewayIp")] public string GatewayIp { get; set; }
		[JsonProperty("lampInfos")] public List<LampInfo> Lamps { get; set; }
	}

	public class LampInfo
	{
		[JsonProperty("deviceUuid")] public string LampId { get; set; }
		[JsonProperty("attributes")] public LampAttributes Attributes { get; set; }
	}

	public class LampAttributes
	{
		[JsonProperty("onOff")] [JsonConverter(typeof(Converters.BooleanConverter))] public bool OnOff { get; set; }
		[JsonProperty("isOnline")] [JsonConverter(typeof(Converters.BooleanConverter))] public bool IsOnline { get; set; }
	}
}
