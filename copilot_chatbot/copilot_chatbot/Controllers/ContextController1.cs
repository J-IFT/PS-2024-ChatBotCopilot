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
    public class ContextController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ContextController(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
            _httpClient.DefaultRequestHeaders.Add("api-key", _configuration["AppSettings:ApiKey"]);
        }

        [HttpPost]
        public async Task<IActionResult> AddContext()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://az-dev-fc-epsi-cog-002-xfq.openai.azure.com/openai/deployments/gpt35/chat/completions?api-version=2024-02-01");
            httpRequest.Content = new StringContent("{\"messages\":[{\"role\":\"user\",\"content\":[{\"type\":\"text\",\"text\":\"" + _configuration["AppSettings:InitialContext"] + "\"}]}], \"temperature\":0.1}", Encoding.UTF8, "application/json");

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
