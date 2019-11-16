using System;

namespace WasteReduction.Models
{
	public class ReceiptInfoCsv
	{
		public ReceiptInfoCsv(string receiptId, DateTime transactionDate, string productId, int quantity)
		{
			ReceiptId = receiptId;
			TransactionDate = transactionDate;
			ProductId = productId;
			Quantity = quantity;
		}

		public string ReceiptId { get; set; }

		public DateTime TransactionDate { get; set; }

		/// <summary>
		/// EAN
		/// </summary>
		public string ProductId { get; set; }

		public int Quantity { get; set; }
	}
}
