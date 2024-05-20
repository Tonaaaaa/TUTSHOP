using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TUTSHOP.Data_Access;
using TUTSHOP.Models.Entities;
using TUTSHOP.Models.Repositories;
using TUTSHOP.Models.ViewModels;

namespace TUTSHOP.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class ProductVariantController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductVariantRepository _productVariantRepository;

		public ProductVariantController(ApplicationDbContext context, IProductVariantRepository productVariantRepository)
        {
            _context = context;
			_productVariantRepository = productVariantRepository;

		}

        // GET: ProductVariant
        public async Task<IActionResult> Index()
        {
            var productVariants = await _context.ProductVariants.Include(pv => pv.Product).ToListAsync();

            // Lặp qua danh sách các biến thể sản phẩm để lấy thông tin về các thuộc tính của mỗi biến thể
            foreach (var variant in productVariants)
            {
                // Lấy danh sách các thuộc tính của biến thể sản phẩm
                variant.VariantsAttributes = await _context.VariantsAttributes
                    .Where(va => va.ProductVariantId == variant.ProductVariantId)
                    .Include(va => va.ProductAttributeValue)
                        .ThenInclude(pav => pav.ProductAttribute)
                    .ToListAsync();
            }

            // Lấy danh sách các thuộc tính
            var attributes = await _context.ProductAttributes.ToListAsync();

            // Truyền danh sách thuộc tính tới view với tên là "AttributeName"
            ViewBag.AttributeName = attributes.Select(attr => attr.ProductAttributeName).ToList();
           
            return View(productVariants);
        }


        // GET: ProductVariant/Create
        public IActionResult Create()
        {
			ViewBag.ProductId = new SelectList(_context.Products.ToList(), "ProductId", "ProductName");
            return View();
        }

        // POST: ProductVariant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productVariant);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Truyền lại dữ liệu cho ViewBag.ProductId trước khi trả về view
            ViewBag.ProductId = new SelectList(_context.Products.ToList(), "ProductId", "ProductName");

            return View(productVariant);
        }

        // GET: ProductVariant/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productVariant = await _productVariantRepository.GetByIdAsync(id);
            if (productVariant == null)
            {
                return NotFound();
            }
            ViewBag.ProductId = new SelectList(_context.Products.ToList(), "ProductId", "ProductName");
            return View(productVariant);
        }

        // POST: ProductVariant/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductVariant productVariant)
        {
            if (id != productVariant.ProductVariantId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
				var existingProductVariant = await _context.ProductVariants
						.FirstOrDefaultAsync(pv => pv.ProductVariantId == id);

				existingProductVariant.Price = productVariant.Price;
				existingProductVariant.Quantity = productVariant.Quantity;
				existingProductVariant.ProductId = productVariant.ProductId;
				existingProductVariant.Color = productVariant.Color;

				_context.Update(productVariant);
				await _context.SaveChangesAsync();
				ViewBag.ProductId = new SelectList(_context.Products.ToList(), "ProductId", "ProductName");
				return RedirectToAction(nameof(Index));
            }
            return View(productVariant);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productVariant = await _context.ProductVariants
                .Include(pv => pv.Product)
                .FirstOrDefaultAsync(m => m.ProductVariantId == id);

            if (productVariant == null)
            {
                return NotFound();
            }

            return View(productVariant);
        }

        // POST: ProductVariant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productVariant = await _context.ProductVariants.FindAsync(id);
            if (productVariant == null)
            {
                return NotFound();
            }

            _context.ProductVariants.Remove(productVariant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductVariantExists(int id)
        {
            return _context.ProductVariants.Any(e => e.ProductVariantId == id);
        }

        public IActionResult GetAttributes(int productId)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
            {
                return NotFound();
            }

            var attributes = _context.ProductAttributes.ToList();
            return PartialView("_ProductAttributes", attributes);
        }
    }
}

