using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmartHome.Services.Network.Base
{
	public class JsonRequest<T> : BaseRequest<T>
	{
		private const string JsonContentType = "application/json";

		private static readonly JsonSerializer _serializer = new JsonSerializer() { NullValueHandling = NullValueHandling.Ignore };
		private static readonly JsonSerializerSettings _ignoreNullSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

		public JsonRequest(Dictionary<string, string> headers) : base(headers)
		{
		}

		private string StringBody
		{
			get
			{
				if (Body == null)
				{
					return string.Empty;
				}

				if (Body is string stringBody)
				{
					return stringBody;
				}
				try
				{
					return JsonConvert.SerializeObject(Body, Formatting.Indented, _ignoreNullSettings);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine("Exception\n" + ex.ToString());
					throw ex;
				}
			}
		}

		protected override HttpRequestMessage GetUniqueRequest(HttpMethod method, Uri url)
		{
			var encodedUrl = Uri.EscapeUriString(url.AbsoluteUri);

			var request = AddHeaders(new HttpRequestMessage(method, encodedUrl));

			if (StringBody != null)
			{
				var content = new StringContent(StringBody, Encoding.UTF8, JsonContentType);
				request.Content = content;
			}

			return request;
		}

		protected async override Task<T> ProcessResponseAsync(HttpResponseMessage responseMessage, CancellationToken token)
		{
			if (responseMessage == null)
			{
				return default(T);
			}

			return await Task.Run(async () =>
			{
				responseMessage.EnsureSuccessStatusCode();

				using (var stream = await responseMessage.Content.ReadAsStreamAsync())
				{
					token.ThrowIfCancellationRequested();

					using (var reader = new StreamReader(stream))
					{
						using (var json = new JsonTextReader(reader))
						{
							token.ThrowIfCancellationRequested();

							return _serializer.Deserialize<T>(json);
						}
					}
				}
			}, token);
		}
	}
}
