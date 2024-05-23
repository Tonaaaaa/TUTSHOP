using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;
using TUTSHOP.Models.Entities;
using TUTSHOP.Models.Repositories;


namespace TUTSHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public AdminProductController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName");
            return View();
        }

		[HttpPost]
		public async Task<IActionResult> Create(Product product, IFormFile imageUrl)
		{
			if (ModelState.IsValid)
			{
				// Tính toán giá trị Price dựa trên OldPrice và DiscountId
				if (product.OldPrice > 0 && product.DiscountId.HasValue && product.DiscountId > 0)
				{
					// Chuyển đổi DiscountId thành phần trăm và tính toán
					float discount = (float)product.DiscountId.Value / 100;
					product.Price = product.OldPrice - (product.OldPrice * discount);
				}
				else
				{
					product.Price = product.OldPrice; // Nếu không có DiscountId, giá sẽ là OldPrice
				}

				product.DateCreated = DateTime.Now;
				product.DateModified = DateTime.Now;

				if (imageUrl != null)
				{
					product.ImageUrl = await SaveImage(imageUrl);
				}

				await _productRepository.AddAsync(product);
				return RedirectToAction(nameof(Index));
			}

			var categories = await _categoryRepository.GetAllAsync();
			ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName");
			return View(product);
		}


		public async Task<IActionResult> Edit(int id)
		{
			var product = await _productRepository.GetByIdAsync(id);
			if (product == null)
			{
				return NotFound();
			}
			var categories = await _categoryRepository.GetAllAsync();
			ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);
			return View(product);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Product product, IFormFile imageUrl)
		{
			if (ModelState.IsValid)
			{
                if (product.OldPrice > 0 && product.DiscountId.HasValue && product.DiscountId > 0)
                {
                    // Chuyển đổi DiscountId thành phần trăm và tính toán
                    float discount = (float)product.DiscountId.Value / 100;
                    product.Price = product.OldPrice - (product.OldPrice * discount);
                }
                else
                {
                    product.Price = product.OldPrice; // Nếu không có DiscountId, giá sẽ là OldPrice
                }
                product.DateCreated = DateTime.Now;
                product.DateModified = DateTime.Now;
                if (imageUrl != null)
				{
					product.ImageUrl = await SaveImage(imageUrl);
				}
				
				await _productRepository.UpdateAsync(product);
				return RedirectToAction(nameof(Index));
			}
			var categories = await _categoryRepository.GetAllAsync();
			ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName");
			return View();
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

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var savePath = Path.Combine("wwwroot/images", image.FileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + image.FileName;
        }

        
    }
}