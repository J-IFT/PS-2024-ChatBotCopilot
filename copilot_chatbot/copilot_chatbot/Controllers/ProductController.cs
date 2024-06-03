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

// File d'attente pour stocker les fichiers en attente de traitement
private readonly Queue<IFormFile> _fileQueue = new Queue<IFormFile>();
private readonly object _lock = new object(); // Verrou pour synchroniser l'accès à la file d'attente

// Action pour soumettre un fichier à la file d'attente
public async Task AddFileToQueue(IFormFile file)
{
    lock (_lock)
    {
        _fileQueue.Enqueue(file);
    }
        Console.WriteLine($"Fichier ajouté à la file d'attente : {file.FileName}");
    await HandleFileQueue();
}

// Méthode pour traiter les fichiers dans la file d'attente
public async Task HandleFileQueue()
{
    while (true)
    {
        IFormFile file;
        lock (_lock)
        {
            if (_fileQueue.Count == 0)
            {
                Console.WriteLine("File d'attente vide.");
                break; // Sortir de la boucle si la file d'attente est vide
            }
            file = _fileQueue.Dequeue(); // Obtenir le prochain fichier de la file d'attente
        }
        Console.WriteLine($"Traitement du fichier de la file d'attente : {file.FileName}");
        // Traiter le fichier
        await UploadFile(file);
    }
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
            Console.WriteLine($"dans uploadfile");

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
                        Blooming_season = row.ContainsKey("A") ? row["A"].ToString() : null,
                        Color = row.ContainsKey("B") ? row["B"].ToString() : null,
                        Exposition = row.ContainsKey("C") ? row["C"].ToString() : null,
                        Last_updated = DateTime.Now,
                        Size = row.ContainsKey("D") ? row["D"].ToString() : null,
                        Name = row["E"].ToString(),
                        Species = row["F"].ToString(),
                        Type = row.ContainsKey("G") ? row["G"].ToString() : null,
                        
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

               
                // Appel de la fonction de génération des données pour ce produit
                await GenerateData(product, importRecord);
                }
    
                // Save changes after adding all products and import records
                await _context.SaveChangesAsync();
                
                await NotifyBotImportCompleted();

                return Ok("Fichier Excel importé avec succès.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Une erreur s'est produite lors de l'import du fichier : {ex.Message}");
        }
    }


private async Task<GeneratedDataProduct> GenerateData(copilot_chatbot.Models.Product product, Import importRecord)
{
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

    // Créez un objet GeneratedDataProduct avec les données générées
    var generatedData = new GeneratedDataProduct
    {
        Title = title,
        Description = description,
        Created_at = DateTime.Now,
        ProductKeywords = keywords
    };

    _context.GeneratedDataProducts.Add(generatedData);
    await _context.SaveChangesAsync();

     // Mise à jour de IsProcessed à true après sauvegarde des données générées
        importRecord.IsProcessed = true;
        _context.Imports.Update(importRecord);
        await _context.SaveChangesAsync();

    return generatedData;

}

private List<ProductKeyword> ExtractKeywords(string keywordsContent)
{
    // Supposons que les mots-clés sont séparés par des virgules dans la réponse
    var keywords = keywordsContent.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(k => k.Trim())
                                  .Distinct()
                                  .ToList();

    var productKeywords = new List<ProductKeyword>();
    foreach (var keyword in keywords)
    {
        var existingKeyword = _context.Keywords.FirstOrDefault(k => k.Name == keyword);
        if (existingKeyword == null)
        {
            existingKeyword = new Keyword { Name = keyword };
            _context.Keywords.Add(existingKeyword);
        }
        productKeywords.Add(new ProductKeyword { Keyword = existingKeyword });
    }

    return productKeywords;
}

   private async Task NotifyBotImportCompleted()
    {
        var message = "L'importation du fichier Excel est terminée et les données ont été traitées avec succès.";
        await _openAIService.GenerateContentAsync(message);
    }

       public class ChatRequest
    {
        public string Message { get; set; }
    }
}
