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
            var allOrders = _orderRepository.GetAll().ToList();
            decimal totalRevenue = 0;
            int totalApprovedOrders = 0;
            int totalPendingOrders = 0;
            int totalCancelledOrders = 0;
            int totalUnpaidOrders = 0;

            var dailyRevenue = new Dictionary<DateTime, decimal>();
            var monthlyRevenue = new Dictionary<string, decimal>();

            foreach (var order in allOrders)
            {
                if (order.IsApproved && (order.PaymentMethod == "VNPay" || order.OrderStatus == OrderStatus.Paid))
                {
                    foreach (var detail in order.OrderDetails)
                    {
                        var orderRevenue = detail.Price * detail.Quantity;
                        totalRevenue += orderRevenue;

                        var orderDate = order.OrderDate.Date;
                        if (dailyRevenue.ContainsKey(orderDate))
                        {
                            dailyRevenue[orderDate] += orderRevenue;
                        }
                        else
                        {
                            dailyRevenue[orderDate] = orderRevenue;
                        }

                        var monthYear = order.OrderDate.ToString("MM/yyyy");
                        if (monthlyRevenue.ContainsKey(monthYear))
                        {
                            monthlyRevenue[monthYear] += orderRevenue;
                        }
                        else
                        {
                            monthlyRevenue[monthYear] = orderRevenue;
                        }
                    }
                }

                if (order.IsApproved)
                {
                    totalApprovedOrders++;
                    if (order.OrderStatus != OrderStatus.Paid)
                    {
                        totalUnpaidOrders++;
                    }
                }

                if (order.OrderStatus == OrderStatus.Cancelled)
                {
                    totalCancelledOrders++;
                }
                else if (!order.IsApproved)
                {
                    totalPendingOrders++;
                }
            }

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalApprovedOrders = totalApprovedOrders;
            ViewBag.TotalPendingOrders = totalPendingOrders;
            ViewBag.TotalCancelledOrders = totalCancelledOrders;
            ViewBag.TotalUnpaidOrders = totalUnpaidOrders;
            ViewBag.DailyRevenueLabels = dailyRevenue.Keys.Select(d => d.ToString("dd/MM/yyyy")).ToList();
            ViewBag.DailyRevenueData = dailyRevenue.Values.ToList();
            ViewBag.MonthlyRevenueLabels = monthlyRevenue.Keys.ToList();
            ViewBag.MonthlyRevenueData = monthlyRevenue.Values.ToList();

            return View();
        }

        [HttpGet]
        public IActionResult SearchOrders(string searchTerm)
        {
            var orders = _orderRepository.GetAll()
                .Where(o => o.ApplicationUser.FullName.Contains(searchTerm) && o.IsApproved)
                .Select(o => new {
                    o.Id,
                    UserFullName = o.ApplicationUser.FullName,
                    UserPhoneNumber = o.ApplicationUser.PhoneNumber,
                    OrderDate = o.OrderDate.ToString("dd/MM/yyyy"),
                    TotalPrice = o.TotalPrice.ToString("N0"),
                    OrderStatus = o.OrderStatus.ToString()
                })
                .ToList();

            return Json(orders);
        }

        public IActionResult ExportInvoice(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Include(o => o.ApplicationUser)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.OrderStatus != OrderStatus.Paid)
            {
                return BadRequest("Chỉ có thể xuất hóa đơn cho những đơn hàng đã thanh toán.");
            }

            var invoicesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "invoices");
            if (!Directory.Exists(invoicesPath))
            {
                Directory.CreateDirectory(invoicesPath);
            }

            var fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "Arial.ttf");
            var fileName = $"Invoice_Order_{order.Id}.pdf";
            var filePath = Path.Combine(invoicesPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                var writer = new PdfWriter(stream);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                var font = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);
                var titleFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H);

                // Add Order Information
                document.Add(new Paragraph("Hóa đơn chi tiết đơn hàng").SetFont(titleFont).SetTextAlignment(TextAlignment.CENTER).SetFontSize(20));
                document.Add(new Paragraph($"Mã đơn hàng: {order.Id}").SetFont(font).SetFontSize(14));
                document.Add(new Paragraph($"Người dùng: {order.ApplicationUser.FullName}").SetFont(font).SetFontSize(14));
                document.Add(new Paragraph($"Ngày đặt: {order.OrderDate.ToString("dd/MM/yyyy")}").SetFont(font).SetFontSize(14));
                document.Add(new Paragraph($"Tổng tiền: {order.TotalPrice.ToString("N0")} đ").SetFont(font).SetFontSize(14));
                document.Add(new Paragraph($"Địa chỉ giao hàng: {order.ShippingAddress}").SetFont(font).SetFontSize(14));
                document.Add(new Paragraph($"Số điện thoại: {order.ApplicationUser.PhoneNumber}").SetFont(font).SetFontSize(14));
                document.Add(new Paragraph($"Ghi chú: {order.Notes}").SetFont(font).SetFontSize(14));
                document.Add(new Paragraph($"Phương thức thanh toán: {order.PaymentMethod}").SetFont(font).SetFontSize(14));
                document.Add(new Paragraph($"Trạng thái đơn hàng: {order.OrderStatus}").SetFont(font).SetFontSize(14));

                document.Add(new Paragraph("\n"));

                // Add Product Details
                document.Add(new Paragraph("Chi tiết sản phẩm").SetFont(font).SetFontSize(16).SetBold());
                var table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 2, 2, 2 })).UseAllAvailableWidth();
                table.AddHeaderCell(new Cell().Add(new Paragraph("Sản phẩm").SetFont(font)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Giá đơn lẻ").SetFont(font)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Số lượng").SetFont(font)));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Thành tiền").SetFont(font)));

                foreach (var item in order.OrderDetails)
                {
                    table.AddCell(new Cell().Add(new Paragraph(item.Product.ProductName).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph((item.Price / item.Quantity).ToString("N0") + " đ").SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(item.Quantity.ToString()).SetFont(font)));
                    table.AddCell(new Cell().Add(new Paragraph(item.Price.ToString("N0") + " đ").SetFont(font)));
                }

                document.Add(table);
                document.Close();
            }

            return File($"/invoices/{fileName}", "application/pdf", fileName);
        }


    }
}
