using System;
using System.Collections.Generic;

namespace WasteReduction.Models.Entities
{
	public class Receipt
	{
		public string ReceiptId { get; set; }

		public DateTime TransactionDate { get; set; }

		public double? Price { get; set; }

		public List<ReceiptItem> ReceiptItems { get; set; }
	}
}
