using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using copilot_chatbot.Models;
using Newtonsoft.Json;

namespace copilot_chatbot.Controllers
{
    public class ChatController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ChatController(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
            _httpClient.DefaultRequestHeaders.Add("api-key", _configuration["AppSettings:ApiKey"]);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] PromptRequest request)
        {
            string prompt = request.prompt;
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://az-dev-fc-epsi-cog-002-xfq.openai.azure.com/openai/deployments/gpt35/chat/completions?api-version=2024-02-01");
            httpRequest.Content = new StringContent("{\"messages\":[{\"role\":\"system\",\"content\":[{\"type\":\"text\",\"text\":\""+ _configuration["AppSettings:InitialContext"] +"\"}]}, {\"role\":\"user\",\"content\":[{\"type\":\"text\",\"text\":\""+prompt+"\"}]}], \"temperature\":0.1}", Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var output = JsonConvert.DeserializeObject<Rootobject>(responseBody);
                return Ok(output);
            }
            else
            {
                return BadRequest("Error");
            }
        }
    }
}
