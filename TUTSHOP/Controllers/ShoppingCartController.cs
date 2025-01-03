using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TUTSHOP.Data_Access;
using TUTSHOP.Extensions;
using TUTSHOP.Models.Entities;
using TUTSHOP.Models.Repositories;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using TUTSHOP.Models.Entities.VNPay;
using Microsoft.EntityFrameworkCore;

namespace TUTSHOP.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVnPayRespository _vnPayRespository;

        public ShoppingCartController(ApplicationDbContext context, IVnPayRespository vnPayRespository, UserManager<ApplicationUser> userManager, IProductRepository productRepository)
        {
            _vnPayRespository = vnPayRespository;
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Checkout()
        {
            return View(new Order());
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var product = await GetProductFromDatabase(productId);
            var cartItem = new CartItem
            {
                ProductId = productId,
                Name = product.ProductName,
                Price = (decimal)product.Price,
                Quantity = quantity,
                ImageUrl = product.ImageUrl
            };
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(cartItem);
            HttpContext.Session.SetObjectAsJson("Cart", cart);

            var cartItemCount = cart.Items.Sum(i => i.Quantity);
            return Json(new { cartItemCount });
        }

        [HttpPost]
        public IActionResult UpdateCart(int productId, int quantity)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            var cartItem = cart.Items.FirstOrDefault(item => item.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);

            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            return View(cart);
        }

        private async Task<Product> GetProductFromDatabase(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            return product;
        }

        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart != null)
            {
                cart.RemoveItem(productId);
                HttpContext.Session.SetObjectAsJson("Cart", cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult RemoveAll()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(Order order, string paymentMethod)
        {
            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");

            if (cart == null || !cart.Items.Any())
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn đang trống.");
                return View(order);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            order.UserId = user.Id;
            order.OrderDate = DateTime.UtcNow;
            order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
            order.OrderDetails = cart.Items.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price, // Giá của một sản phẩm
                Product = _context.Products.Find(i.ProductId)
            }).ToList();
            order.PaymentMethod = paymentMethod;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            if (paymentMethod == "COD")
            {
                HttpContext.Session.Remove("Cart");
                return View("OrderCompleted", order);
            }
            if (paymentMethod == "VNPay")
            {
                var vnPayModel = new VnPaymentRequestModel
                {
                    OrderId = order.Id,
                    Amount = cart.Items.Sum(i => i.Price * i.Quantity),
                    CreatedDate = DateTime.Now,
                    Description = "",
                    FullName = user.FullName
                };
                return Redirect(_vnPayRespository.CreatePaymentUrl(HttpContext, vnPayModel));
            }

            return View("OrderCompleted", order);
        }

        [Authorize]
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayRespository.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }

            var order = _context.Orders.FirstOrDefault(p => p.Id == Convert.ToInt32(response.OrderDescription));
            if (order != null)
            {
                HttpContext.Session.Remove("Cart"); // Xóa giỏ hàng khi thanh toán thành công
            }

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public IActionResult CheckoutSubmit(int deliId, decimal amount, int payment)
        {
            if (ModelState.IsValid)
            {
                if (payment == 2)
                {
                    var vnPayModel = new VnPaymentRequestModel
                    {
                        Amount = amount,
                        CreatedDate = DateTime.Now,
                        Description = "",
                        FullName = "a",
                        OrderId = new Random().Next(1000, 100000)
                    };
                    return Redirect(_vnPayRespository.CreatePaymentUrl(HttpContext, vnPayModel));
                }
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ApplyDiscount(string discountCode)
        {
            var discount = await _context.DiscountCodes.FirstOrDefaultAsync(d => d.Code == discountCode && d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now && d.Quantity > 0);
            if (discount == null)
            {
                return Json(new { success = false, message = "Mã giảm giá không hợp lệ hoặc đã hết hạn." });
            }

            var cart = HttpContext.Session.GetObjectFromJson<ShoppingCart>("Cart");
            if (cart == null || !cart.Items.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng của bạn đang trống." });
            }

            decimal discountAmount = 0;
            if (discount.Type == DiscountType.Percentage)
            {
                discountAmount = cart.Items.Sum(i => i.Price * i.Quantity) * (discount.DiscountAmount / 100);
            }
            else if (discount.Type == DiscountType.FixedAmount)
            {
                discountAmount = discount.DiscountAmount;
            }
            else if (discount.Type == DiscountType.FreeShipping)
            {
                // Handle free shipping logic if applicable
            }

            var newTotalPrice = cart.Items.Sum(i => i.Price * i.Quantity) - discountAmount;

            // Reduce the quantity of the discount code by 1
            discount.Quantity -= 1;
            _context.DiscountCodes.Update(discount);
            await _context.SaveChangesAsync();

            return Json(new { success = true, discountAmount = discountAmount.ToString("N0"), newTotalPrice = newTotalPrice.ToString("N0") });
        }
    }
}