using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SmartHome.Services.Network
{
	public abstract class AbstracttHttpClientFactory : IHttpClientFactory
	{
		private const string GzipAcceptHeader = "gzip";
		private const int DefaultTimeOut = 60;

		private static HttpClient _sharedClient;

		public AbstracttHttpClientFactory()
		{
		}

		public HttpClient GetClient()
		{
			if (_sharedClient == null)
			{
#if DEBUG
				var clientHandler = new LoggingHandler(GetClientHandler(), true);
#else
				var clientHandler = GetClientHandler();
#endif
				_sharedClient = new HttpClient(clientHandler)
				{
					Timeout = TimeSpan.FromSeconds(DefaultTimeOut),
				};

				_sharedClient.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue { NoCache = true };
				_sharedClient.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue(GzipAcceptHeader));
			}

			return _sharedClient;
		}

		protected abstract HttpMessageHandler GetClientHandler();

		private class LoggingHandler : DelegatingHandler
		{
			private bool _enableLogging;

			public LoggingHandler(HttpMessageHandler innerHandler, bool enableLogging)
				: base(innerHandler)
			{
				_enableLogging = enableLogging;
			}

			protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			{
				Log("Url:" + request.RequestUri.AbsoluteUri);
				Log("Request:");
				Log(request.ToString());

				if (request.Content != null)
				{
					Log(await request.Content.ReadAsStringAsync());
				}

				Log("");

				HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

				Log("Response:");
				Log(response.ToString());

				if (response.Content != null)
				{
					Log(await response.Content.ReadAsStringAsync());
				}

				Log("");

				return response;
			}

			private void Log(string text)
			{
				if (_enableLogging)
				{
					Console.WriteLine(text);
				}
			}
		}
	}
}
