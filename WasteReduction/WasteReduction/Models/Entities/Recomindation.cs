namespace WasteReduction.Models.Entities
{
	public class Recomindation
	{
		public int RecomindationId { get; set; }

		public string Title { get; set; }

		public int Quantity { get; set; }

		public double Price { get; set; }

		public string ProductId { get; set; }

		public Product Product { get; set; }
	}
}
