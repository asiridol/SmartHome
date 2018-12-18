using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Text;
using Prism.Ioc;

namespace SmartHome.Services.Network.Base
{
	public abstract class BaseRequest<T>
	{
		private Dictionary<string, string> _headers;

		public BaseRequest(Dictionary<string, string> headers)
		{
			_headers = headers;
		}

		private HttpClient SharedClient
		{
			get
			{
				return Prism.PrismApplicationBase.Current.Container.Resolve<IHttpClientFactory>().GetClient();
			}
		}

		public object Body { get; set; }

		public Task<T> SendRequestAsync(HttpMethod method, Uri url)
		{
			return SendRequestAsync(method, url, CancellationToken.None);
		}

		public async Task<T> SendRequestAsync(HttpMethod method, Uri url, CancellationToken token)
		{
			HttpResponseMessage responseMessage = null;

			if (method == HttpMethod.Get)
			{
				responseMessage = await GetAsync(url, token);
			}
			else if (method == HttpMethod.Delete)
			{
				responseMessage = await DeleteAsync(url, token);
			}
			else if (method == HttpMethod.Post)
			{
				responseMessage = await PostAsync(url, token);
			}
			else if (method == HttpMethod.Put)
			{
				responseMessage = await PutAsync(url, token);
			}
			// HEAD
			else
			{
				throw new NotSupportedException("Head not supported");
			}

			var processedResponse = await ProcessResponseAsync(responseMessage, token);

			return processedResponse;
		}

		private Task<HttpResponseMessage> GetAsync(Uri url, CancellationToken token)
		{
			var request = GetUniqueRequest(HttpMethod.Get, url);
			return SharedClient.SendAsync(request, token);
		}

		private Task<HttpResponseMessage> PostAsync(Uri url, CancellationToken token)
		{
			var request = GetUniqueRequest(HttpMethod.Post, url);
			return SharedClient.SendAsync(request, token);
		}

		private Task<HttpResponseMessage> PutAsync(Uri url, CancellationToken token)
		{
			var request = GetUniqueRequest(HttpMethod.Put, url);
			return SharedClient.SendAsync(request, token);
		}

		private Task<HttpResponseMessage> DeleteAsync(Uri url, CancellationToken token)
		{
			var request = GetUniqueRequest(HttpMethod.Delete, url);
			return SharedClient.SendAsync(request, token);
		}

		protected abstract HttpRequestMessage GetUniqueRequest(HttpMethod method, Uri url);

		protected HttpRequestMessage AddHeaders(HttpRequestMessage requestMessage)
		{
			if (_headers != null && _headers.Any())
			{
				foreach (var header in _headers)
				{
					requestMessage.Headers.Add(header.Key, header.Value);
				}
			}

			return requestMessage;
		}

		protected abstract Task<T> ProcessResponseAsync(HttpResponseMessage responseMessage, CancellationToken token);
	}
}
