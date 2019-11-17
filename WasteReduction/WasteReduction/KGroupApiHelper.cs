using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace WasteReduction
{
	public class KGroupApiHelper
	{
		private static IConfiguration _configuration;

		public KGroupApiHelper(IConfiguration config)
		{
			_configuration = config;
		}

		public async Task<T> MakeGetRequest<T>(string uri)
		{
			T result;

			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(uri);

				if (!response.IsSuccessStatusCode)
					throw new ApplicationException($"Error request with status code: {response.StatusCode}");

				string content = await response.Content.ReadAsStringAsync();

				result = JsonConvert.DeserializeObject<T>(content);
			};

			return result;
		}

		public async Task<TOut> MakePostRequest<TIn, TOut>(TIn request, string uri)
		{
			TOut result;

			using (var client = CreateHttpClient())
			{
				HttpResponseMessage response;

				// Request body
				byte[] byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request).ToString());

				using (var content = new ByteArrayContent(byteData))
				{
					content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
					response = await client.PostAsync(uri, content);
				}

				if (!response.IsSuccessStatusCode)
					throw new ApplicationException($"Error request with status code: {response.StatusCode}");

				string dataString = await response.Content.ReadAsStringAsync();

				result = JsonConvert.DeserializeObject<TOut>(dataString);
			};

			return result;
		}

		private HttpClient CreateHttpClient()
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _configuration["KGroupAPIKey"]);

			return client;
		}
	}
}
