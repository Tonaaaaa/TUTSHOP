using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using TUTSHOP.Models.Entities;
using TUTSHOP.Models.Repositories;

namespace TUTSHOP.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index(string category)
        {
            IEnumerable<Product> products;
            if (string.IsNullOrEmpty(category))
            {
                products = await _productRepository.GetAllAsync();
            }
            else
            {
                string decodedCategory = HttpUtility.UrlDecode(category);
                products = await _productRepository.GetByCategoryAsync(decodedCategory);
            }

            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // Action method cho tìm kiếm
        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return View(new List<Product>());
            }

            var products = await _productRepository.SearchAsync(keyword);
            return View(products);
        }
        // Action method cho tìm kiếm gợi ý (live search)
        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return Json(new List<Product>());
            }

            var products = await _productRepository.SearchAsync(keyword);
            return Json(products.Select(p => new
            {
                id = p.ProductId,
                productName = p.ProductName,
                price = p.Price.ToString("C"),
                oldPrice = p.OldPrice > p.Price ? p.OldPrice.ToString("C") : "",
                imageUrl = p.ImageUrl
            }));
        }
    }
}
