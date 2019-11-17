using Microsoft.EntityFrameworkCore;
using WasteReduction.Models.Entities;

namespace WasteReduction
{
	public class ApplicationDbContext : DbContext
	{
		public DbSet<Receipt> Receipts { get; set; }
		public DbSet<Product> Products { get; set; }

		public DbSet<ReceiptItem> ReceiptItems { get; set; }

		public DbSet<Recomindation> Recomindations { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
	}
}
