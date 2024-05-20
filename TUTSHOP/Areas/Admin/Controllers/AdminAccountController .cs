using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TUTSHOP.Models.Entities;

namespace TUTSHOP.Areas.Admin.Controllers
{
    
        [Area("Admin")]
        [Authorize(Roles = SD.Role_Admin)]
        public class AdminAccountController : Controller
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager; // Thêm RoleManager để quản lý các vai trò

            public AdminAccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
            {
                _userManager = userManager;
                _roleManager = roleManager;
            }

            public async Task<IActionResult> Index()
            {
                // Lấy vai trò Admin từ cơ sở dữ liệu
                var adminRole = await _roleManager.FindByNameAsync("Admin");

                if (adminRole == null)
                {
                    // Xử lý khi không tìm thấy vai trò Admin
                    // Có thể thông báo lỗi hoặc thực hiện hành động phù hợp khác
                    return RedirectToAction("Error", "Home");
                }

                // Lấy danh sách các người dùng thuộc vai trò Admin
                var usersInAdminRole = await _userManager.GetUsersInRoleAsync(adminRole.Name);

                return View(usersInAdminRole);
            }


            public async Task<IActionResult> Customer()
            {
                // Lấy vai trò Admin từ cơ sở dữ liệu
                var customerRole = await _roleManager.FindByNameAsync("Customer");

                if (customerRole == null)
                {
                // Xử lý khi không tìm thấy vai trò Customer
                // Có thể thông báo lỗi hoặc thực hiện hành động phù hợp khác
                return RedirectToAction("Error", "Home");
                }

                // Lấy danh sách các người dùng thuộc vai trò 
                var usersInCustomerRole = await _userManager.GetUsersInRoleAsync(customerRole.Name);

                return View(usersInCustomerRole);
            }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Xóa người dùng từ cơ sở dữ liệu
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Xử lý lỗi nếu có
                // Ví dụ: Hiển thị thông báo lỗi cho người dùng
                return RedirectToAction("Error", "Home");
            }
        }
    }
    }
    

