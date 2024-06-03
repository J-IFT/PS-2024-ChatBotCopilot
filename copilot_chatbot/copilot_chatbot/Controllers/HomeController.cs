using System.Diagnostics;
using copilot_chatbot.Models;
using copilot_chatbot.Utilities;
using copilot_chatbot.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Http;

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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Connexion(string username, string email, string password)
        {
            try
            {
                // Vérifiez si l'utilisateur existe déjà dans la base de données
                var user = _context.Users.FirstOrDefault(u => u.Username == username);

                if (user != null)
                {
                    // L'utilisateur existe déjà, vérifiez le mot de passe
                    var authenticationService = new ConnexionService(_context);
                    if (authenticationService.IsUserValid(username, email, password))
                    {
                        // Si le mot de passe est correct, redirigez l'utilisateur vers la page d'accueil
						HttpContext.Session.SetInt32("UserId", user.Id);
                        return RedirectToAction("Privacy", "Home");
                    }
                    else
                    {
                        // Si le mot de passe est incorrect, affichez un message d'erreur et redirigez l'utilisateur vers la page de connexion
                        ViewBag.ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect.";
                        return View("Index");
                    }
                }
                else
                {
                    // L'utilisateur n'existe pas encore, créez un nouvel utilisateur avec les informations fournies
                    var newUser = new User { Username = username, Email = email, Password = password };
                    _context.Users.Add(newUser);
                    _context.SaveChanges();

                    // Connectez l'utilisateur en le redirigeant vers la page d'accueil
                    return RedirectToAction("Privacy", "Home");
                }
            }
            catch (Exception ex)
            {
                // Gestion des erreurs
                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}