using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace copilot_chatbot.Services
{
    public class OpenAIService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public OpenAIService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            var apiKey = _configuration.GetValue<string>("AppSettings:OpenAI:ApiKey");
            var modelEndpoint = _configuration.GetValue<string>("AppSettings:OpenAI:ModelEndpoint");

            Console.WriteLine($"ApiKey: {apiKey}");
            Console.WriteLine($"ModelEndpoint: {modelEndpoint}");
        }
        public async Task<ResponseModel> GenerateContentAsync(string prompt)
        {
            var apiKey = _configuration.GetValue<string>("AppSettings:OpenAI:ApiKey");
            var modelEndpoint = _configuration.GetValue<string>("AppSettings:OpenAI:ModelEndpoint");

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(modelEndpoint))
            {
                throw new InvalidOperationException("API Key or Model Endpoint is not configured");
            }

            var request = new HttpRequestMessage(HttpMethod.Post, modelEndpoint);
            request.Headers.Add("api-key", apiKey);

            var body = new
            {
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful assistant." },
                    new { role = "user", content = prompt }
                }
            };

            var jsonBody = JsonConvert.SerializeObject(body);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<ResponseModel>(jsonResponse);

                if (responseModel == null)
                {
                    throw new InvalidOperationException("Response from OpenAI is null");
                }

                return responseModel;
            }
            else
            {
                throw new HttpRequestException($"Failed to get response from OpenAI: {response.ReasonPhrase}");
            }
        }
        public bool ContainsKeyword(string message, string keyword)
        {
            return message.Contains(keyword, StringComparison.OrdinalIgnoreCase);
        }
    }
    public class ResponseModel
    {
        public List<Choice> choices { get; set; }
        public long created { get; set; }
        public string id { get; set; }
        public string model { get; set; }
        public string @object { get; set; }
        public List<PromptFilterResult> prompt_filter_results { get; set; }
        public string system_fingerprint { get; set; }
        public Usage usage { get; set; }
    }

    public class Choice
    {
        public ContentFilterResults content_filter_results { get; set; }
        public string finish_reason { get; set; }
        public int index { get; set; }
        public object logprobs { get; set; }
        public Message message { get; set; }
    }

    public class ContentFilterResults
    {
        public Hate hate { get; set; }
        public SelfHarm self_harm { get; set; }
        public Sexual sexual { get; set; }
        public Violence violence { get; set; }
    }

    public class Hate
    {
        public bool filtered { get; set; }
        public string severity { get; set; }
    }

    public class SelfHarm
    {
        public bool filtered { get; set; }
        public string severity { get; set; }
    }

    public class Sexual
    {
        public bool filtered { get; set; }
        public string severity { get; set; }
    }

    public class Violence
    {
        public bool filtered { get; set; }
        public string severity { get; set; }
    }

    public class Message
    {
        public string content { get; set; }
        public string role { get; set; }
    }

    public class PromptFilterResult
    {
        public int prompt_index { get; set; }
        public ContentFilterResults content_filter_results { get; set; }
    }

    public class Usage
    {
        public int completion_tokens { get; set; }
        public int prompt_tokens { get; set; }
        public int total_tokens { get; set; }
    }
}
