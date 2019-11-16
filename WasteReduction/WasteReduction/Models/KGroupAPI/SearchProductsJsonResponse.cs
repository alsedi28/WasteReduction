using Newtonsoft.Json;

namespace WasteReduction.Models.KGroupAPI
{
	public class SearchProductsJsonResponse
	{
		[JsonProperty("totalHits")]
		public int TotalItems { get; set; }

		[JsonProperty("results")]
		public Product[] Results { get; set; }
	}

	public class Product
	{
		[JsonProperty("labelName")]
		public ProductNames Names { get; set; }

		[JsonProperty("pictureUrls")]
		public PictureUrl[] Pictures { get; set; }

		[JsonProperty("ean")]
		public string Ean { get; set; }

		[JsonProperty("attributes")]
		public ProductAttributes Attributes { get; set; }
	}

	public class ProductNames
	{
		[JsonProperty("english")]
		public string EnglishName { get; set; }

		[JsonProperty("finnish")]
		public string FinnishName { get; set; }
	}

	public class PictureUrl
	{
		[JsonProperty("original")]
		public string Url { get; set; }
	}

	public class ProductAttributes
	{
		[JsonProperty("WHERL")]
		public ProductAttribute ManufacturerCountry { get; set; }
	}

	public class ProductAttribute
	{
		[JsonProperty("value")]
		public ProductAttributeValue Value { get; set; }
	}

	public class ProductAttributeValue
	{
		[JsonProperty("value")]
		public string Value { get; set; }
	}
}
