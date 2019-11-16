using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WasteReduction.Models.Entities
{
	public class Product
	{
		public string ProductId { get; set; }

		public string Name { get; set; }

		public string PictureUrl { get; set; }

		public string ManufacturerCountry { get; set; }

		public string Co2 { get; set; }

		public bool isWasted { get; set; }

		public bool isFinished { get; set; }

		[JsonIgnore]
		public List<ReceiptItem> ReceiptItems { get; set; }
	}
}
