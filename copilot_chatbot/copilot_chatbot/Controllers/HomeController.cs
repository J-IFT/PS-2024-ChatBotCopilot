using System.Diagnostics;
using copilot_chatbot.Models;
using copilot_chatbot.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace copilot_chatbot.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ExcelManager _excelManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _excelManager = new ExcelManager();
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _excelManager.ReadExcel();  // Ceci retourne une liste de copilot_chatbot.Utilities.Product
            return View(products);  // Passer directement la liste à la vue
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //example of db use
        [HttpPost]
        public IActionResult AddDataToDatabase()
        {
            try
            {
                var newUser = new User { Username = "john_doe", Email = "john@example.com" };
                _context.Users.Add(newUser);
                _context.SaveChanges();

                return Ok("Données ajoutées avec succès à la base de données.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }
    }
}