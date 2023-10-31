using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TestController(/*ICard card*/ IHttpClientFactory httpClientFactory)
        {
            //_card = card;
            _httpClientFactory = httpClientFactory;

        }

        [HttpGet]
        [Route("FilterdatabasedonNameAndColor")]
        public async Task<ActionResult<IEnumerable<Card>>> GetCardsAsync([FromQuery] string name, [FromQuery] string color )
        {
            try
            {
                string apiUrl = "https://api.magicthegathering.io/v1/cards";
                var queryParameters = new List<string>();

                if (!string.IsNullOrEmpty(name))
                    queryParameters.Add($"name={Uri.EscapeDataString(name)}");

                if (!string.IsNullOrEmpty(color))
                    queryParameters.Add($"colors={Uri.EscapeDataString(color)}");
    

                    if (queryParameters.Count > 0)
                    apiUrl += "?" + string.Join('&', queryParameters);

                
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetStringAsync(apiUrl);

                
                var apiResponse = JsonSerializer.Deserialize<CardApiResponse>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse != null && apiResponse.Cards != null)
                {
                    
                    var filteredCards = apiResponse.Cards
                        .Where(card => IsCardTypeMatch(card, "Creature")) 
                        .ToList();

                    return Ok(filteredCards);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
               
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }
        bool IsCardTypeMatch(Card card, string desiredType)
        {
            if (card.Types != null)
            {
                foreach (var type in card.Types)
                {
                    if (string.Equals(type, desiredType, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [HttpGet]
        [Route("GEtALLData")]
        public async Task<ActionResult<IEnumerable<Card>>> GetCardsdataAsync()
        {
            try
            {
                string apiUrl = "https://api.magicthegathering.io/v1/cards";
                var queryParameters = new List<string>();
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetStringAsync(apiUrl);

              
                var apiResponse = JsonSerializer.Deserialize<CardApiResponse>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse != null && apiResponse.Cards != null && apiResponse.Cards.Any())
                {
                    return Ok(apiResponse.Cards);
                }
                else
                {
                    return NotFound("No cards found for the specified criteria.");
                }
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }




    }
}
