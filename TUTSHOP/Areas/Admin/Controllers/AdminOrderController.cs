using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TUTSHOP.Models.Entities;
using TUTSHOP.Models.Repositories;
using TUTSHOP.Data_Access; // Thêm dòng này để sử dụng ApplicationDbContext
using System.Linq;

namespace TUTSHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context; // Thêm ApplicationDbContext

        public AdminOrderController(IOrderRepository orderRepository, ApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context; // Khởi tạo ApplicationDbContext
        }

        public IActionResult Index()
        {
            var orders = _orderRepository.GetAll();
            return View(orders);
        }

        public IActionResult ApprovedOrders()
        {
            var approvedOrders = _orderRepository.GetAll().Where(o => o.IsApproved).ToList();
            return View(approvedOrders);
        }

        public IActionResult PendingOrders()
        {
            var pendingOrders = _orderRepository.GetAll().Where(o => !o.IsApproved).ToList();
            return View(pendingOrders);
        }

        public IActionResult Details(int id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            // Lấy lại thông tin từ cơ sở dữ liệu để đảm bảo tính nhất quán
            foreach (var detail in order.OrderDetails)
            {
                var product = _context.Products.Find(detail.ProductId);
                detail.Product = product;
                detail.Price = detail.Price * detail.Quantity; // Giá tổng cộng của sản phẩm
            }

            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, OrderStatus status)
        {
            _orderRepository.UpdateOrderStatus(id, status);
            return RedirectToAction(nameof(ApprovedOrders));
        }

        public IActionResult ApproveOrder(int id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            order.IsApproved = true;
            _orderRepository.UpdateOrderStatus(id, OrderStatus.Processing); // Khi duyệt đơn hàng, đặt trạng thái là "Processing"
            return RedirectToAction(nameof(PendingOrders));
        }

        public IActionResult TotalRevenue()
        {
            /*var approvedOrders = _orderRepository.GetAll().Where(o => o.IsApproved).ToList();
            decimal totalRevenue = 0;

            foreach (var order in approvedOrders)
            {
                foreach (var detail in order.OrderDetails)
                {
                    var product = _context.Products.Find(detail.ProductId);
                    if (product != null)
                    {
                        totalRevenue += (decimal)product.Price * (decimal)detail.Quantity;


                    }
                }
            }

            ViewBag.TotalRevenue = totalRevenue;
            return View();*/

            var allOrders = _orderRepository.GetAll().ToList();
            decimal totalRevenue = 0;
            int totalApprovedOrders = 0;
            int totalPendingOrders = 0;

            foreach (var order in allOrders)
            {
                // Tính tổng doanh thu của các đơn hàng đã được duyệt
                if (order.IsApproved)
                {
                    foreach (var detail in order.OrderDetails)
                    {
                        var product = _context.Products.Find(detail.ProductId);
                        if (product != null)
                        {
                            totalRevenue += (decimal)product.Price * (decimal)detail.Quantity;
                        }
                    }
                    totalApprovedOrders += order.OrderDetails.Sum(detail => detail.Quantity);
                }
                else
                {
                    // Tính tổng số lượng đơn hàng chưa được duyệt
                    totalPendingOrders += order.OrderDetails.Sum(detail => detail.Quantity);
                }
            }

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalApprovedOrders = totalApprovedOrders;
            ViewBag.TotalPendingOrders = totalPendingOrders;
            return View();
        }
    }
}
