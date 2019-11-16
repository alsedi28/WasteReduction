using System;
using System.Net.Http;
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

		private HttpClient CreateHttpClient()
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _configuration["KGroupAPIKey"]);

			return client;
		}
	}
}
