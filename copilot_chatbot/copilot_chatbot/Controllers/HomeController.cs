using System.Diagnostics;
using copilot_chatbot.Models;
using copilot_chatbot.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace copilot_chatbot.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ExcelManager _excelManager;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _excelManager = new ExcelManager();
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
    }
}