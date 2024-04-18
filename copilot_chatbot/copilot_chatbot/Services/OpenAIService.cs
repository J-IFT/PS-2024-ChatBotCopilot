using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;

namespace copilot_chatbot.Services
{
    public class OpenAIService
    {
        private const string apiKey = "538bebe57f1c4fd7a3167b21335ad5a3";
        private const string modelEndpoint = "https://az-dev-fc-epsi-cog-002-xfq.openai.azure.com/completion";

        public ResponseModel GenerateContent(string prompt)
        {
            var client = new RestClient(modelEndpoint);
            var request = new RestRequest("POST");  // Utilisation de la chaîne de caractères "POST" pour spécifier la méthode

            request.AddHeader("Authorization", $"Bearer {apiKey}");
            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                prompt,
                max_tokens = 100
            };

            request.AddJsonBody(body);

            var response = client.Execute(request);

            if (response.IsSuccessful)
            {
                var jsonResponse = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
                return jsonResponse;
            }
            else
            {
                // Gérer l'erreur
                return null;
            }
        }
    }

    public class ResponseModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
    }
}
