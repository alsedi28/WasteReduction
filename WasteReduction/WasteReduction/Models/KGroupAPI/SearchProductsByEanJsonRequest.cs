using Newtonsoft.Json;
using System.Collections.Generic;

namespace WasteReduction.Models.KGroupAPI
{
	public class SearchProductsByEanJsonRequest
	{
		[JsonProperty("filters")]
		public Filter Filters { get; set; }
	}

	public class Filter
	{
		[JsonProperty("ean")]
		public List<string> Ean { get; set; }
	}
}
