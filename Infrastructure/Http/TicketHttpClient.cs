using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamsMS.Application.DTOs.Request;
using StreamsMS.Domain.Exceptions;
using System.Text;
using System.Text.Json;

namespace StreamsMS.Infrastructure.Http
{
    public class TicketHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public TicketHttpClient(HttpClient httpClient, IConfiguration config)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> UseTicket(UseTicketRequest request)
        {
            var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var apiKey = _config["APIKEY:TICKETS"];

            if(!string.IsNullOrEmpty(apiKey)) 
                _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);

            var response = await _httpClient.PostAsync("/api/v1/tickets/use", jsonContent);

            if(!response.IsSuccessStatusCode)
            {
                var body = await  response.Content.ReadAsStringAsync();
                var message = JsonConvert.DeserializeObject<JObject>(body);
                throw new BusinessRuleException($"{message["message"]}");
            }

            return response;
        }
    }
}
