using System.IO;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TUTSHOP.Models.Entities;
using TUTSHOP.Models.Repositories;
using TUTSHOP.Data_Access;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using iText.IO.Font;

namespace TUTSHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;

        public AdminOrderController(IOrderRepository orderRepository, ApplicationDbContext context)
        {
            _orderRepository = orderRepository;
            _context = context;
        }

        public IActionResult Index()
        {
            var orders = _orderRepository.GetAll();
            return View(orders);
        }

        public IActionResult ApprovedOrders()
        {
            var approvedOrders = _orderRepository.GetAll().Where(o => o.IsApproved).ToList();

            foreach (var order in approvedOrders)
            {
                if (order.PaymentMethod == "VNPay" && order.OrderStatus != OrderStatus.Paid)
                {
                    order.OrderStatus = OrderStatus.Paid;
                    _orderRepository.UpdateOrderStatus(order.Id, OrderStatus.Paid);
                }
            }

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

            foreach (var detail in order.OrderDetails)
            {
                var product = _context.Products.Find(detail.ProductId);
                detail.Product = product;
                detail.Price = detail.Price * detail.Quantity;
            }

            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderStatus = newStatus;
            _context.SaveChanges();

            return RedirectToAction("ApprovedOrders");
        }

        public IActionResult ApproveOrder(int id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
            {
                return NotFound();
            }

            order.IsApproved = true;
            _orderRepository.UpdateOrderStatus(id, OrderStatus.Processing);
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
