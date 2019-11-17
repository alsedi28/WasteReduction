using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using WasteReduction.Models;
using WasteReduction.Models.Entities;
using Product = WasteReduction.Models.Entities.Product;
using WasteReduction.Models.KGroupAPI;
using Microsoft.EntityFrameworkCore;
using WasteReduction.Models.Responses;

namespace WasteReduction.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DataController : ControllerBase
	{
		private readonly KGroupApiHelper _kGroupApiHelper;
		private readonly ApplicationDbContext _applicationDbContext;
		private IHostingEnvironment _hostingEnvironment;

		public DataController(KGroupApiHelper kGroupApiHelper, ApplicationDbContext applicationDbContext, IHostingEnvironment hostingEnvironment)
		{
			_kGroupApiHelper = kGroupApiHelper;
			_applicationDbContext = applicationDbContext;
			_hostingEnvironment = hostingEnvironment;
		}

		[HttpGet("products")]
		public async Task<IActionResult> GetProductsAsync(string name, int amount)
		{
			var request = new SearchProductsByQueryJsonRequest
			{
				Query = name,
				View = new ViewField
				{
					Limit = amount
				}
			};

			var response = await _kGroupApiHelper.MakePostRequest<SearchProductsByQueryJsonRequest, SearchProductsJsonResponse>(request, "https://kesko.azure-api.net/v1/search/products");

			var products = response.Results.Select(p => new Product
			{
				ProductId = p.Ean,
				Name = string.IsNullOrEmpty(p.Names.EnglishName) ? p.Names.FinnishName : p.Names.EnglishName,
				PictureUrl = p.Pictures.FirstOrDefault()?.Url,
				ManufacturerCountry = p.Attributes.ManufacturerCountry.Value.Value
			}).ToList();

			return Ok(new GetResponse<List<Product>> { Result = products });
		}

		[HttpGet("allproducts")]
		public async Task<IActionResult> GetAllProductsAsync()
		{
			var request = new SearchProductsByEanJsonRequest
			{
				Filters = new Filter
				{
					Ean = _applicationDbContext.Products.Select(p => p.ProductId).Distinct().ToList()
				}
			};

			var response = await _kGroupApiHelper.MakePostRequest<SearchProductsByEanJsonRequest, SearchProductsJsonResponse>(request, "https://kesko.azure-api.net/v1/search/products");

			var products = response.Results.Select(p => new Product
			{
				ProductId = p.Ean,
				Name = string.IsNullOrEmpty(p.Names.EnglishName) ? p.Names.FinnishName : p.Names.EnglishName,
				PictureUrl = p.Pictures.FirstOrDefault()?.Url,
				ManufacturerCountry = p.Attributes.ManufacturerCountry.Value.Value
			}).ToList();

			return Ok(new GetResponse<List<Product>> { Result = products });
		}

		[HttpGet("receipts")]
		public async Task<IActionResult> GetReceipts()
		{
			var receipts = _applicationDbContext.Receipts
				.Include(r => r.ReceiptItems)
				.ThenInclude(r => r.Product)
				.ToList();

			var request = new SearchProductsByEanJsonRequest
			{
				Filters = new Filter
				{
					Ean = receipts.SelectMany(r => r.ReceiptItems).Select(r => r.ProductId).Distinct().ToList()
				}
			};

			var response = await _kGroupApiHelper.MakePostRequest<SearchProductsByEanJsonRequest, SearchProductsJsonResponse>(request, "https://kesko.azure-api.net/v1/search/products");

			foreach(var product in receipts.SelectMany(r => r.ReceiptItems.Select(i => i.Product)))
			{
				var tempProduct = response.Results.FirstOrDefault(p => p.Ean == product.ProductId);

				if (tempProduct == null)
					continue;

				product.ManufacturerCountry = tempProduct.Attributes.ManufacturerCountry.Value.Value;
				product.Name = string.IsNullOrEmpty(tempProduct.Names.EnglishName) ? tempProduct.Names.FinnishName : tempProduct.Names.EnglishName;
				product.PictureUrl = tempProduct.Pictures.FirstOrDefault()?.Url;
			}

			return Ok(new GetResponse<List<Receipt>> { Result = receipts });
		}

		[HttpGet("products/{id}/recomindation")]
		[Obsolete("Method on Stub")]
		public IActionResult GetRecomindationForProduct(string id)
		{
			var result = new Recomindation
			{
				RecomindationId = 1,
				Title = "Test recomindation",
				Quantity = 3,
				ProductId = "ABC",
				Product = new Product
				{
					ProductId = "ABC",
					Name = "Product",
					PictureUrl = "ssss"
				}
			};

			return Ok(new GetResponse<Recomindation> { Result = result });
		}

		[HttpGet("recomindations")]
		[Obsolete("Method on Stub")]
		public IActionResult GetRecomindations()
		{
			var result = new List<Recomindation>
			{
				new Recomindation
				{
					RecomindationId = 1,
					Title = "Test recomindation",
					Quantity = 3,
					ProductId = "ABC",
					Product = new Product
					{
						ProductId = "ABC",
						Name = "Product",
						PictureUrl = "ssss"
					}
				},
				new Recomindation
				{
					RecomindationId = 1,
					Title = "Test recomindation",
					Quantity = 3,
					ProductId = "ABC",
					Product = new Product
					{
						ProductId = "ABC",
						Name = "Product",
						PictureUrl = "ssss"
					}
				}
			};

			return Ok(new GetResponse<List<Recomindation>> { Result = result });
		}

		[HttpPost("ParseReceiptData")]
		[NonAction]
		public IActionResult ParseReceiptData()
		{
			string path = Path.Combine(_hostingEnvironment.ContentRootPath, "Data\\ReceiptData.csv");
			var receiptsCsvInfo = new List<ReceiptInfoCsv>();

			using (var sr = new StreamReader(path, System.Text.Encoding.Default))
			{
				string line = sr.ReadLine();

				while ((line = sr.ReadLine()) != null)
				{
					var lineParts = line.Split(';');

					receiptsCsvInfo.Add(new ReceiptInfoCsv(lineParts[1], Convert.ToDateTime(lineParts[2]), lineParts[4], Int32.Parse(lineParts[5])));
				}
			}

			var receiptGroup = receiptsCsvInfo.GroupBy(r => r.ReceiptId);

			var products = receiptsCsvInfo.Select(p => p.ProductId).Distinct().Select(p => new Product { ProductId = p });
			_applicationDbContext.Products.AddRange(products);
			_applicationDbContext.SaveChanges();

			foreach (var group in receiptGroup)
			{
				var receipt = new Receipt();

				receipt.ReceiptId = group.Key;
				receipt.TransactionDate = group.First().TransactionDate;

				receipt.ReceiptItems = group.Select(g => new ReceiptItem
				{
					Quantity = g.Quantity,
					ProductId = g.ProductId
				}).ToList();

				_applicationDbContext.Receipts.Add(receipt);
				_applicationDbContext.SaveChanges();
			}

			return Ok();
		}
	}
}
