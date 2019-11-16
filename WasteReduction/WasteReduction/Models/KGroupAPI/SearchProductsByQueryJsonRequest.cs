using Newtonsoft.Json;

namespace WasteReduction.Models.KGroupAPI
{
	public class SearchProductsByQueryJsonRequest
	{
		[JsonProperty("query")]
		public string Query { get; set; }

		[JsonProperty("view")]
		public ViewField View { get; set; }
	}

	public class ViewField
	{
		[JsonProperty("limit")]
		public int Limit { get; set; }
	}
}
