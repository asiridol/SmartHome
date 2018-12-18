using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartHome.Services.Network.Models
{
	public class AuthenticationRequest
	{
		[JsonProperty("user")] public string User { get; set; }
		[JsonProperty("pwd")] public string Passwod { get; set; }
		[JsonProperty("uuid")] public string Uuid { get; set; }
		[JsonProperty("osType")] public string OsType { get; set; }
		[JsonProperty("productCode")] public string ProductCode { get; set; }
		[JsonProperty("appCode")] public string AppCode { get; set; }
	}
}
