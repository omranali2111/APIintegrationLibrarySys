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
        public static string AuthToken;
        public OpsController()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("https://localhost:7166/");
        }






        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            Log.Information($"AuthToken value: {AuthToken}");
            // Check if the token is available
            if (string.IsNullOrEmpty(AuthToken))
            {
                return Unauthorized("Token is missing or invalid.");
            }

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", AuthToken);

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
        public async Task<IActionResult> Login(string email, string password)
        {
            var loginRequest = new { Email = email, Password = password };

            try
            {
                // Log received credentials
                Log.Information($"Received login request - Email: {email}, Password: {password}");

                HttpResponseMessage response = await Client.PostAsJsonAsync("api/UserLogin/user-login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var tokenResponse = await response.Content.ReadAsStringAsync();
                    AuthToken = tokenResponse; // Store the token as JSON string

                    return Ok(new { Token = tokenResponse }); // Return JSON response
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
