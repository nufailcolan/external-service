using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReqresApi.Client.Model;
using System.Net;
using System.Text.Json;
using TestAssignment.IService;
using TestAssignment.Model;

namespace TestAssignment.Service
{
    public class ExternalUserService : IExternalUserService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalUserService> _logger;
        private readonly string _baseUrl;

        public ExternalUserService(HttpClient httpClient, IConfiguration configuration, ILogger<ExternalUserService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _baseUrl = configuration["ReqresApi:BaseUrl"];
        }



        public async Task<UserData> GetUserByIdAsync(int userId)
        {

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/users/{userId}");


            requestMessage.Headers.Add("x-api-key", "reqres-free-v1");

            var response = await _httpClient.SendAsync(requestMessage);
            var json = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var wrapper = JsonSerializer.Deserialize<JsonElement>(json);
                var user = JsonSerializer.Deserialize<UserData>(wrapper.GetProperty("data").ToString());
                return user;
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            else if (!response.IsSuccessStatusCode)
            {
                var exception = JsonSerializer.Deserialize<ExceptionModel>(json);
                throw new Exception($"Error occured while calling the API {requestMessage.RequestUri}: {exception.error}");
            }

            throw new Exception("Failed to fetch user.");
        }


        public async Task<List<UserData>> GetAllUsersAsync()
        {
            var result = new User();
            int page = 2;


            var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/users?page={page}");


            requestMessage.Headers.Add("x-api-key", "reqres-free-v1");

            var response = await _httpClient.SendAsync(requestMessage);




            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var exception = JsonSerializer.Deserialize<ExceptionModel>(json);
                throw new Exception($"Error occured while calling the API {requestMessage.RequestUri}: {exception.error}");
            }

            result = JsonSerializer.Deserialize<User>(json);


            return result.data;
        }
    }
}
