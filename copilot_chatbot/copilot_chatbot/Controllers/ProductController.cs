using copilot_chatbot.Services;
using copilot_chatbot.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using ClosedXML.Excel;
using Newtonsoft.Json;
using copilot_chatbot.Models;

public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly OpenAIService _openAIService;
    private readonly ExcelManager _excelManager;
    private readonly IConfiguration _configuration;

    public ProductController(ApplicationDbContext context, OpenAIService openAIService, ExcelManager excelManager, IConfiguration configuration)
    {
        _context = context;
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

            //Question 1 pptx : Je souhaite générer de nouvelles descriptions
            if (_openAIService.ContainsKeyword(userMessage, "générer"))
            {
                return Ok(new { message = "Bien sûr, importez votre fichier Excel et dites moi quand c’est fait" });
            }
            //Question 2 pptx : Fichier xxxx.xlsx importé | Voilà, c’est importé
            else if (_openAIService.ContainsKeyword(userMessage, "importé"))
            {
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

    [HttpPost("UploadFile")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Aucun fichier sélectionné.");
            }

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Veuillez sélectionner un fichier Excel (.xlsx).");
            }

            using (var workbook = new XLWorkbook(file.OpenReadStream()))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed().Skip(1).ToList(); // Skip header row
                var data = new List<Dictionary<string, object>>();

                foreach (var row in rows)
                {
                    var rowData = new Dictionary<string, object>();
                    foreach (var cell in row.Cells())
                    {
                        rowData[cell.Address.ColumnLetter] = cell.Value;
                    }
                    data.Add(rowData);
                }

                var jsonData = JsonConvert.SerializeObject(data);
                Console.WriteLine(jsonData);

                // Insert the imported data into the database
                foreach (var row in data)
                {
                    var product = new copilot_chatbot.Models.Product
                    {
                        Name = row["A"].ToString(),
                        Species = row["B"].ToString(),
                        Type = row.ContainsKey("C") ? row["C"].ToString() : null,
                        Size = row.ContainsKey("D") ? row["D"].ToString() : null,
                        Blooming_season = row.ContainsKey("E") ? row["E"].ToString() : null,
                        Color = row.ContainsKey("F") ? row["F"].ToString() : null,
                        Exposition = row.ContainsKey("G") ? row["G"].ToString() : null,
                        Last_updated = DateTime.Now
                    };
                    _context.Products.Add(product);

                    var importRecord = new Import
                    {
                        IsProcessed = false,
                        Imported_at = DateTime.Now,
                        UserId = 1, // en dur, à passer en dynamique
                        Product = product
                    };
                    _context.Imports.Add(importRecord);
                }

                // Save changes after adding all products and import records
                await _context.SaveChangesAsync();

                return Ok("Fichier Excel importé avec succès.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur s'est produite lors de l'import du fichier : {ex.Message}");
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }
}
