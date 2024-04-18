using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;

namespace copilot_chatbot.Services
{
    public class OpenAIService
    {
        private readonly IConfiguration _configuration;

        public OpenAIService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ResponseModel GenerateContent(string prompt)
        {
            var apiKey = _configuration["AppSettings:OpenAI:ApiKey"];
            var modelEndpoint = _configuration["AppSettings:OpenAI:ModelEndpoint"];

            var client = new RestClient(modelEndpoint);
            var request = new RestRequest("POST");

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
