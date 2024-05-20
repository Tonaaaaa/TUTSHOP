using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TUTSHOP.Models.Entities;
using TUTSHOP.Models.Repositories;

namespace TUTSHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminProductVariantController : Controller
    {
        private readonly IProductVariantRepository _productVariantRepository;

        public AdminProductVariantController(IProductVariantRepository productVariantRepository)
        {
            _productVariantRepository = productVariantRepository;
        }

        public async Task<IActionResult> Index()
        {
            var productVariants = await _productVariantRepository.GetAllAsync();
            return View(productVariants);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                await _productVariantRepository.AddAsync(productVariant);
                return RedirectToAction(nameof(Index));
            }
            return View(productVariant);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var productVariant = await _productVariantRepository.GetByIdAsync(id);
            if (productVariant == null)
            {
                return NotFound();
            }
            return View(productVariant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductVariant productVariant)
        {
            if (ModelState.IsValid)
            {
                await _productVariantRepository.UpdateAsync(productVariant);
                return RedirectToAction(nameof(Index));
            }
            return View(productVariant);
        }

        public async Task<IActionResult> Details(int id)
        {
            var productVariant = await _productVariantRepository.GetByIdAsync(id);
            if (productVariant == null)
            {
                return NotFound();
            }
            return View(productVariant);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var productVariant = await _productVariantRepository.GetByIdAsync(id);
            if (productVariant == null)
            {
                return NotFound();
            }
            return View(productVariant);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productVariantRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
