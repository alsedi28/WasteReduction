using System.Text.Json.Serialization;

namespace WasteReduction.Models.Entities
{
	public class ReceiptItem
	{
		public int ReceiptItemId { get; set; }

		public int Quantity { get; set; }

		public double Price { get; set; }

		public string ProductId { get; set; }

		public Product Product { get; set; }

		[JsonIgnore]
		public string ReceiptId { get; set; }

		[JsonIgnore]
		public Receipt Receipt { get; set; }
	}
}
