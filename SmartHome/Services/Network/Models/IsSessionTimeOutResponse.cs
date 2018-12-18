using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net;

namespace SmartHome.Services.Network.Models
{
	public class IsSessionTimeOutResponse
	{
		[JsonProperty("messageCode")] [JsonConverter(typeof(StringEnumConverter))] public HttpStatusCode StatusCode { get; set; }
		[JsonProperty("info")] public string Info { get; set; }
		[JsonProperty("description")] public string Description { get; set; }
		[JsonProperty("snapServiceMinVerison")] public long SnapServiceMinVerison { get; set; }
		[JsonProperty("ifCheckSessionid")] public long IfCheckSessionid { get; set; }
	}
}
