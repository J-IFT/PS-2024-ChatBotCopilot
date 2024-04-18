using copilot_chatbot.Services;
using copilot_chatbot.Utilities;
using Microsoft.AspNetCore.Mvc;

public class ProductController : Controller
{
    private readonly OpenAIService _openAIService;
    private readonly ExcelManager _excelManager;

    public ProductController(OpenAIService openAIService, ExcelManager excelManager)
    {
        _openAIService = openAIService;
        _excelManager = excelManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ChatWithBot(string message)
    {
        var userState = HttpContext.Session.GetString("UserState");

        if (string.IsNullOrEmpty(userState))
        {
            userState = "Initial";
        }

        switch (userState)
        {
            case "Initial":
                if (message.ToLower().Contains("générer de nouvelles descriptions"))
                {
                    HttpContext.Session.SetString("UserState", "Import");
                    return Content("Bot: Bien sûr, importez votre fichier Excel et dites moi quand c’est fait");
                }
                break;

            case "Import":
                if (message.ToLower().Contains("fichier"))
                {
                    var products = _excelManager.ReadExcel();
                    if (products != null && products.Any())
                    {
                        HttpContext.Session.SetString("UserState", "Generate");
                        return Content("Bot: J’ai lancé l’import, revenez dans quelques minutes quand ce sera terminé");
                    }
                    else
                    {
                        return Content("Bot: Aucun produit à traiter");
                    }
                }
                break;

            case "Generate":
                if (message.ToLower().Contains("export"))
                {
                    var products = _excelManager.ReadExcel();
                    foreach (var product in products)
                    {
                        var prompt = $"Générer un titre, une description et des mots clés pour le produit {product.Name} avec les caractéristiques {product.Features}.";
                        var response = _openAIService.GenerateContent(prompt);
                        product.Title = response.Title;
                        product.Description = response.Description;
                        product.Tags = response.Tags;
                    }

                    _excelManager.WriteExcel(products);

                    HttpContext.Session.SetString("UserState", "Initial");
                    return Content("Bot: Bien sûr, voici un export des données");
                }
                break;

            default:
                break;
        }

        return Content("Bot: Je ne comprends pas votre demande");
    }
}
