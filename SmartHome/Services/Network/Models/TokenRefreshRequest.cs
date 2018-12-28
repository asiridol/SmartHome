using System;
using Newtonsoft.Json;

namespace SmartHome.Services.Network.Models
{
	public class TokenRefreshRequest
	{
		[JsonProperty("uuid")] public string Uuid { get; set; }
		[JsonProperty("osType")] public string OsType { get; set; }
		[JsonProperty("appCode")] public string AppCode { get; set; }
	}
}
