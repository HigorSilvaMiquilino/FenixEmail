using System.Diagnostics;
using emaildisparator.Models;
using emaildisparator.Service.Home;
using FenixEmail.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace emaildisparator.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HomeService _homeService;



        public HomeController(ILogger<HomeController> logger, HomeService homeService)
        {
            _logger = logger;
            _homeService = homeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Disparo()
        {
            return View(await _homeService.GetAllAsync());
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistrarUserModel model)
        {
            if (ModelState.IsValid)
            {
                await _homeService.CreateAsync(model);
                return RedirectToAction(nameof(Disparo));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmails(List<string> selectedEmails)
        {
            if (selectedEmails != null && selectedEmails.Any())
            {
                var cleanedEmails = selectedEmails
                        .Where(email => !string.IsNullOrEmpty(email))
                        .Distinct()
                        .ToList();

                await _homeService.dispararTodosEmailsAsync(cleanedEmails);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            await _homeService.DeleteUserAsync(userId);
            return Ok();
        }   


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
