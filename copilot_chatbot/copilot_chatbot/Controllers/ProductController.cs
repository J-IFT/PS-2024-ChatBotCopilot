using copilot_chatbot.Services;
using copilot_chatbot.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

public class ProductController : Controller
{
    private readonly OpenAIService _openAIService;
    private readonly ExcelManager _excelManager;
    private readonly IConfiguration _configuration;

    public ProductController(OpenAIService openAIService, ExcelManager excelManager, IConfiguration configuration)
    {
        _openAIService = openAIService ?? throw new ArgumentNullException(nameof(openAIService));
        _excelManager = excelManager ?? throw new ArgumentNullException(nameof(excelManager));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ChatWithBot([FromBody] ChatRequest request)
    {
        try
        {
            var response = await _openAIService.GenerateContentAsync(request.Message);
            var userMessage = request.Message.ToLower();

            if (_openAIService.ContainsKeyword(userMessage, "import"))
            {
                return Ok(new { message = "Bien sûr, importez votre fichier Excel et dites moi quand c’est fait" });
            }
            else if (_openAIService.ContainsKeyword(userMessage, "fichier"))
            {
                // Ajouter le code pour l'importation de l'Excel ici
                return Ok(new { message = "J’ai lancé l’import, revenez dans quelques minutes quand ce sera terminé" });
            }
            else
            {
                var assistantMessage = response.choices?.FirstOrDefault()?.message?.content;

                if (assistantMessage != null && !_openAIService.ContainsKeyword(userMessage, "import") && !_openAIService.ContainsKeyword(userMessage, "fichier"))
                {
                    return Ok(new { message = assistantMessage });
                }
                else if (assistantMessage == null)
                {
                    throw new InvalidOperationException("Response from OpenAI does not contain a message content");
                }
            }
        }
        catch (Exception ex)
        {
            // Log l'exception pour le débogage
            Console.WriteLine($"Erreur lors de l'envoi du message : {ex.Message}");
            return BadRequest("Erreur lors de l'envoi du message : " + ex.Message);
        }
        return Ok();
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }
}
