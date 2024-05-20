using Microsoft.AspNetCore.Mvc;
using TUTSHOP.Data_Access;
using TUTSHOP.Models.Entities;
using TUTSHOP.Models.Repositories;

namespace TUTSHOP.Controllers
{
	public class ContactController : Controller
	{
		private readonly ApplicationDbContext _context;



		private readonly IContactRepository _contactService;

		public ContactController(IContactRepository contactService)
		{
			_contactService = contactService;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Index(Contact model)
		{
			if (ModelState.IsValid)
			{
				var message = new Contact
				{
					ContactName = model.ContactName,
					Phone = model.Phone,
					Mail = model.Mail,
					Message = model.Message,
					DateCreated = DateTime.Now
				};

				_contactService.SaveMessage(message);

				// Redirect to a thank you page or show a success message
				return RedirectToAction("ThankYou");
			}

			return View(model);
		}

		public ActionResult ThankYou()
		{
			return View();
		}
	}
}
