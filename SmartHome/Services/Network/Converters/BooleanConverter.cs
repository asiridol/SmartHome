using System;
using Newtonsoft.Json;
namespace SmartHome.Services.Network.Converters
{
	public class BooleanConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(bool) || objectType == typeof(string);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var value = reader.Value;

			if (value == null || string.IsNullOrEmpty(value.ToString()))
			{
				return false;
			}

			if (bool.TryParse(value.ToString(), out var result))
			{
				return result;
			}

			if (int.TryParse(value.ToString(), out var intValue))
			{
				return intValue == 1;
			}

			return false;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}
	}
}
