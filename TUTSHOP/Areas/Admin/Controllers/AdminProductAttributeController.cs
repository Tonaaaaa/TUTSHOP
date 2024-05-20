using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TUTSHOP.Data_Access;
using TUTSHOP.Models.Entities;

namespace TUTSHOP.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class AdminProductAttributeController : Controller
	{
		private readonly ApplicationDbContext _context;

		public AdminProductAttributeController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Admin/AdminProductAttribute
		public async Task<IActionResult> Index()
		{
			return View(await _context.ProductAttributes.ToListAsync());
		}

		// GET: Admin/AdminProductAttribute/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var productAttribute = await _context.ProductAttributes
				.FirstOrDefaultAsync(m => m.ProductAttributeId == id);
			if (productAttribute == null)
			{
				return NotFound();
			}

			return View(productAttribute);
		}

		// GET: Admin/AdminProductAttribute/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Admin/AdminProductAttribute/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("ProductAttributeId,ProductAttributeName")] ProductAttribute productAttribute)
		{
			if (ModelState.IsValid)
			{
				_context.Add(productAttribute);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(productAttribute);
		}

		// GET: Admin/AdminProductAttribute/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var productAttribute = await _context.ProductAttributes.FindAsync(id);
			if (productAttribute == null)
			{
				return NotFound();
			}
			return View(productAttribute);
		}

		// POST: Admin/AdminProductAttribute/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("ProductAttributeId,ProductAttributeName")] ProductAttribute productAttribute)
		{
			if (id != productAttribute.ProductAttributeId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(productAttribute);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProductAttributeExists(productAttribute.ProductAttributeId))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			return View(productAttribute);
		}

		// GET: Admin/AdminProductAttribute/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var productAttribute = await _context.ProductAttributes
				.FirstOrDefaultAsync(m => m.ProductAttributeId == id);
			if (productAttribute == null)
			{
				return NotFound();
			}

			return View(productAttribute);
		}

		// POST: Admin/AdminProductAttribute/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var productAttribute = await _context.ProductAttributes.FindAsync(id);
			_context.ProductAttributes.Remove(productAttribute);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ProductAttributeExists(int id)
		{
			return _context.ProductAttributes.Any(e => e.ProductAttributeId == id);
		}
	}
}
