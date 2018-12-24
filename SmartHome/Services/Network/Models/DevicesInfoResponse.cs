using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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
	}
}
