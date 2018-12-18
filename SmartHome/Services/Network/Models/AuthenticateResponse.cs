using System;
using Newtonsoft.Json;

namespace SmartHome.Services.Network.Models
{
	public class AuthenticateResponse
	{
		[JsonProperty("ret")] public long Ret { get; set; }
		[JsonProperty("msg")] public string Msg { get; set; }
		[JsonProperty("customerId")] public long CustomerId { get; set; }
		[JsonProperty("mobileId")] public long MobileId { get; set; }
		[JsonProperty("nick_name")] public string NickName { get; set; }
		[JsonProperty("relative_path")] public Uri RelativePath { get; set; }
		[JsonProperty("jsessionId")] public string JsonPropertysessionId { get; set; }
	}
}
