using APIintegrationLibrarySys.models;
using Auth0.ManagementApi.Models.Rules;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace APIintegrationLibrarySys.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpsController : ControllerBase
    {
        HttpClient Client;
        private string AuthToken;
        public OpsController()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("https://localhost:7166/");
        }






        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            // Check if the token is available
            if (string.IsNullOrEmpty(AuthToken))
            {
                return Unauthorized("Token is missing or invalid.");
            }

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthToken);

            try
            {
                HttpResponseMessage resp = await Client.GetAsync("api/BookOperation/ViewAllBooks");

                if (resp.IsSuccessStatusCode)
                {
                    string responseContent = await resp.Content.ReadAsStringAsync();
                    List<Book> books = JsonConvert.DeserializeObject<List<Book>>(responseContent);
                    return Ok(books);
                }
                else
                {
                    return BadRequest($"Failed to retrieve books. Status code: {resp.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginRequest)
        {
            try
            {
                // Log received credentials
                Log.Information($"Received login request - Email: {loginRequest.Email}, Password: {loginRequest.Password}");

                HttpResponseMessage response = await Client.PostAsJsonAsync("api/UserLogin/user-login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    AuthToken = await response.Content.ReadAsStringAsync(); // Store the token
                    return Ok(AuthToken);
                }
                else
                {
                    Log.Warning($"Login failed - Status code: {response.StatusCode}");
                    return Unauthorized("Invalid credentials");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred during login: {ex.Message}");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }
}
