using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SmartHome.Services.Network.Models
{
	public class PropertyValue
	{
		[JsonProperty("switch")]
		public int Switch { get; set; }
		[JsonProperty("gradientTime")]
		public int GradientTime => 10;
		[JsonProperty("deviceUuidList")]
		public List<string> LampsList { get; set; }
	}

	public class LightTriggerRequest
	{
		[JsonProperty("dn")]
		public string HubId { get; set; }
		[JsonProperty("type")]
		public string Property => "groupSwitch";
		[JsonProperty("value")]
		public PropertyValue Value { get; set; }
		[JsonProperty("time")]
		public double TimeStamp { get; set; }
	}
}
