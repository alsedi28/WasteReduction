using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WasteReduction.Models.KGroupAPI;
using ProductResult = WasteReduction.Models.Product;

namespace WasteReduction.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class DataController : ControllerBase
	{
		private readonly KGroupApiHelper _kGroupApiHelper;

		public DataController(KGroupApiHelper kGroupApiHelper)
		{
			_kGroupApiHelper = kGroupApiHelper;
		}

		[HttpGet("products")]
		public async Task<IActionResult> GetAsync(string name, int amount)
		{
			var request = new SearchProductsJsonRequest
			{
				Query = name,
				View = new ViewField
				{
					Limit = amount
				}
			};

			var response = await _kGroupApiHelper.MakePostRequest<SearchProductsJsonRequest, SearchProductsJsonResponse>(request, "https://kesko.azure-api.net/v1/search/products");

			var products = response.Results.Select(p => new ProductResult
			{
				Ean = p.Ean,
				Name = string.IsNullOrEmpty(p.Names.EnglishName) ? p.Names.FinnishName : p.Names.EnglishName,
				PictureUrl = p.Pictures.FirstOrDefault()?.Url,
				ManufacturerCountry = p.Attributes.ManufacturerCountry.Value.Value
			}).ToList();

			return Ok(products);
		}
	}
}
