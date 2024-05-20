using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using TUTSHOP.Models.Entities;
using TUTSHOP.Models.Repositories;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace TUTSHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminDiscountIdController : Controller
    {
        private readonly IDiscountCodeRepository _discountCodeRepository;

        public AdminDiscountIdController(IDiscountCodeRepository discountCodeRepository)
        {
            _discountCodeRepository = discountCodeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var discountCodes = await _discountCodeRepository.GetAllAsync();
            return View(discountCodes);
        }

        public IActionResult Create()
        {
            // Lấy danh sách loại giảm giá để hiển thị trong dropdownlist
            var discountTypes = Enum.GetValues(typeof(DiscountType))
                                    .Cast<DiscountType>()
                                    .Select(x => new SelectListItem
                                    {
                                        Text = x.GetDisplayName(),
                                        Value = ((int)x).ToString()
                                    }).ToList();

            ViewBag.DiscountTypes = discountTypes;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DiscountCode discountCode)
        {
            if (ModelState.IsValid)
            {
                // Xử lý khi ModelState hợp lệ
                await _discountCodeRepository.AddAsync(discountCode);
                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, cần tái tạo danh sách loại giảm giá cho dropdownlist
            var discountTypes = Enum.GetValues(typeof(DiscountType))
                                    .Cast<DiscountType>()
                                    .Select(x => new SelectListItem
                                    {
                                        Text = x.GetDisplayName(),
                                        Value = ((int)x).ToString()
                                    }).ToList();

            ViewBag.DiscountTypes = discountTypes;

            return View(discountCode);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var discountCode = await _discountCodeRepository.GetByIdAsync(id);
            if (discountCode == null)
            {
                return NotFound();
            }
            return View(discountCode);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, DiscountCode discountCode)
        {
            if (id != discountCode.DiscountCodeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Xử lý khi ModelState hợp lệ
                await _discountCodeRepository.UpdateAsync(discountCode);
                return RedirectToAction(nameof(Index));
            }
            return View(discountCode);
        }

        public async Task<IActionResult> Details(int id)
        {
            var discountCode = await _discountCodeRepository.GetByIdAsync(id);
            if (discountCode == null)
            {
                return NotFound();
            }
            return View(discountCode);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var discountCode = await _discountCodeRepository.GetByIdAsync(id);
            if (discountCode == null)
            {
                return NotFound();
            }
            return View(discountCode);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _discountCodeRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
