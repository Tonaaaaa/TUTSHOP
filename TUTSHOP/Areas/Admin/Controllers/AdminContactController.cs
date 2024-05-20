using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TUTSHOP.Models.Entities;
using TUTSHOP.Models.Repositories;

namespace TUTSHOP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AdminContactController : Controller
    {
        private readonly IContactRepository _contactService;

        public AdminContactController(IContactRepository contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var contact = await _contactService.GetAllAsync();
            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Contact model)
        {
            if (ModelState.IsValid)
            {
                var message = new Contact
                {
                    ContactName = model.ContactName,
                    Mail = model.Mail,
                    Phone = model.Phone,
                    Message = model.Message


                };

                _contactService.SaveMessage(message);

                // Redirect to a thank you page or show a success message
                return RedirectToAction("ThankYou");
            }

            return View(model);
        }
    }
}
