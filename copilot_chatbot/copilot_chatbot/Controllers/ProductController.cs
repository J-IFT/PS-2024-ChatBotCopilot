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
            //Question 3 pptx : Fais moi un export des références produit
            else if (_openAIService.ContainsKeyword(userMessage, "export"))
            {
                return Ok(new { message = "Bien sûr, voici un export des données" });
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
                                Console.WriteLine($"dans uploadfile");

                    var product = new copilot_chatbot.Models.Product
                    {   
                        Blooming_season = row.ContainsKey("A") ? row["A"].ToString() : null,
                        Color = row.ContainsKey("B") ? row["B"].ToString() : null,
                        Exposition = row.ContainsKey("C") ? row["C"].ToString() : null,
                        Last_updated = DateTime.Now,
                        Size = row.ContainsKey("D") ? row["D"].ToString() : null,
                        Name = row["E"].ToString(),
                        Species = row["F"].ToString(),
                        Type = row.ContainsKey("G") ? row["G"].ToString() : null
                    
                    };
                    _context.Products.Add(product);

                    var importRecord = new Import
                    {
                        IsProcessed = false,
                        Imported_at = DateTime.Now,
                        UserId = 1,//à modifier selon le user, à rendre dynamique
                        Product = product
                    };
                    _context.Imports.Add(importRecord);

                    // Appel de la fonction de génération des données pour ce produit
                    await GenerateData(product, importRecord);
                }
                // Save changes after adding all products and import records
                await _context.SaveChangesAsync();

                await NotifyBotImportCompleted();

                // Return a message indicating successful import
                return Ok(new { message = "Fichier Excel importé avec succès." });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur s'est produite lors de l'import du fichier : {ex.Message}");
        }
    }

    private async Task<GeneratedDataProduct> GenerateData(copilot_chatbot.Models.Product product, Import importRecord)
    {
        Console.WriteLine($"Début de la génération de données pour le produit : {product.Name}");

        // Générer le titre
        var titleResponse = await _openAIService.GenerateContentAsync($"Générer un titre pour le produit {product.Name}. Caractéristiques : Blooming_season: {product.Blooming_season}, Color: {product.Color}, Exposition: {product.Exposition}, Size: {product.Size}, Species: {product.Species}, Type: {product.Type}");
        var title = titleResponse.choices?.FirstOrDefault()?.message?.content ?? $"Titre généré pour {product.Name}";

        // Générer la description
        var descriptionResponse = await _openAIService.GenerateContentAsync($"Générer une description pour le produit {product.Name}. Caractéristiques : Blooming_season: {product.Blooming_season}, Color: {product.Color}, Exposition: {product.Exposition}, Size: {product.Size}, Species: {product.Species}, Type: {product.Type}");
        var description = descriptionResponse.choices?.FirstOrDefault()?.message?.content ?? "Description générée";

        // Générer les mots-clés
        var keywordsResponse = await _openAIService.GenerateContentAsync($"Générer 5 mots-clés pour le produit {product.Name}. Caractéristiques : Blooming_season: {product.Blooming_season}, Color: {product.Color}, Exposition: {product.Exposition}, Size: {product.Size}, Species: {product.Species}, Type: {product.Type}");
        var keywordsContent = keywordsResponse.choices?.FirstOrDefault()?.message?.content;
        var keywords = ExtractKeywords(keywordsContent);

        // Enregistrement des données générées dans la base de données
        var generatedData = new GeneratedDataProduct
        {
            Title = title,
            Description = description,
            Created_at = DateTime.Now,
            ProductKeywords = keywords,
            //ProductId = à rendre dynamique
            //ExportId = à rendre dynamique
        };

        _context.GeneratedDataProducts.Add(generatedData);
        await _context.SaveChangesAsync();

        // Mise à jour de IsProcessed à true après sauvegarde des données générées
        importRecord.IsProcessed = true;
        _context.Imports.Update(importRecord);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Données générées et enregistrées pour le produit : {product.Name}");
        return generatedData;
    }

    private List<ProductKeyword> ExtractKeywords(string keywordsContent)
    {
        Console.WriteLine($"Extraction des mots-clés à partir du contenu : {keywordsContent}");
        // Supposons que les mots-clés sont séparés par des virgules dans la réponse
        var keywords = keywordsContent.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                          .Select(k => k.Trim())
                                          .Distinct()
                                          .ToList();

        var productKeywords = new List<ProductKeyword>();
        foreach (var keyword in keywords)
        {
        
            Console.WriteLine($"Traitement du mot-clé : {keyword}");
            var existingKeyword = _context.Keywords.FirstOrDefault(k => k.Name == keyword);
            if (existingKeyword == null)
            {
                Console.WriteLine($"Création d'un nouveau mot-clé : {keyword}");
                existingKeyword = new Keyword { Name = keyword };
                _context.Keywords.Add(existingKeyword);
            }
            productKeywords.Add(new ProductKeyword { Keyword = existingKeyword });
        }

        Console.WriteLine($"Mots-clés extraits et enregistrés : {string.Join(", ", keywords)}");
        return productKeywords;
    }

    private async Task NotifyBotImportCompleted()
    {
        Console.WriteLine("Début de la notification d'importation terminée");
        var message = "L'importation du fichier Excel est terminée et les données ont été traitées avec succès.";
        await _openAIService.GenerateContentAsync(message);
        Console.WriteLine("Notification d'importation terminée envoyée");
    }

    [HttpPost("ExportReferences")]
    [HttpGet("ExportReferences")]
    public async Task<IActionResult> ExportReferences()
    {
        try
        {
            Console.WriteLine("ExportReferences method called.");
            // Étape 2 : Récupérer toutes les références produits depuis la base de données
            var references = _context.Products.ToList(); // À adapter selon votre modèle de données
            Console.WriteLine($"Total products: {references.Count}"); // Ajout d'un message de débogage
            // Étape 3 : Parser les données JSON en Excel
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("References");
                var currentRow = 1;

                // Ajouter des en-têtes de colonne
                worksheet.Cell(currentRow, 1).Value = "Blooming_season";
                worksheet.Cell(currentRow, 2).Value = "Color";
                worksheet.Cell(currentRow, 3).Value = "Exposition";
                worksheet.Cell(currentRow, 4).Value = "Last_updated";
                worksheet.Cell(currentRow, 5).Value = "Name";
                worksheet.Cell(currentRow, 6).Value = "Size";
                worksheet.Cell(currentRow, 7).Value = "Species";
                worksheet.Cell(currentRow, 8).Value = "Type";
                Console.WriteLine("En têtes done");
                // Ajouter les données des références
                foreach (var reference in references)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = reference.Blooming_season;
                    worksheet.Cell(currentRow, 2).Value = reference.Color;
                    worksheet.Cell(currentRow, 3).Value = reference.Exposition;
                    worksheet.Cell(currentRow, 4).Value = reference.Last_updated;
                    worksheet.Cell(currentRow, 5).Value = reference.Name;
                    worksheet.Cell(currentRow, 6).Value = reference.Size;
                    worksheet.Cell(currentRow, 7).Value = reference.Species;
                    worksheet.Cell(currentRow, 8).Value = reference.Type;
                }
                Console.WriteLine("Refs done");
                // Sauvegarder le fichier Excel dans un MemoryStream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    // Retournez le fichier Excel au client
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "references.xlsx");
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur s'est produite lors de l'export des références : {ex.Message}");
        }
    }


    public class ChatRequest
    {
        public string Message { get; set; }
    }
}
