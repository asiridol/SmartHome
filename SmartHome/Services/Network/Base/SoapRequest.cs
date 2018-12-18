using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SmartHome.Services.Network.Base
{
	public class SoapRequest<T> : BaseRequest<T>
	{
		private const string SoapContentType = "text/xml";

		public SoapRequest(Dictionary<string, string> headers) : base(headers)
		{
		}

		private String StringBody
		{
			get
			{
				if (Body == null)
				{
					return string.Empty;
				}

				XmlSerializer xsSubmit = new XmlSerializer(Body.GetType());
				var xml = "";

				using (var sww = new StringWriter())
				{
					using (XmlWriter writer = XmlWriter.Create(sww))
					{
						xsSubmit.Serialize(writer, Body);
						xml = sww.ToString(); // Your XML
					}
				}

				return xml;
			}
		}

		protected override HttpRequestMessage GetUniqueRequest(HttpMethod method, Uri url)
		{
			var encodedUrl = Uri.EscapeUriString(url.AbsoluteUri);

			var request = AddHeaders(new HttpRequestMessage(method, encodedUrl));

			if (StringBody != null)
			{
				var content = new StringContent(StringBody, Encoding.UTF8, SoapContentType);
				request.Content = content;
			}

			return request;
		}

		protected override Task<T> ProcessResponseAsync(HttpResponseMessage responseMessage, CancellationToken token)
		{
			throw new NotImplementedException();
		}
	}
}
