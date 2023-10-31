using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace test
{
    public class Card
    {
        public string Name { get; set; }
        public string ManaCost { get; set; }
        public double CMC { get; set; }
        public List<string> Colors { get; set; }
        public List<string> ColorIdentity { get; set; }
        public string Type { get; set; }
        public List<string> Types { get; set; }
        public List<string> Subtypes { get; set; }
        public string Rarity { get; set; }
        public string Set { get; set; }
        public string SetName { get; set; }
        public string Text { get; set; }
        public string Artist { get; set; }
        public string Number { get; set; }
        public string Power { get; set; }
        public string Toughness { get; set; }
        public string Layout { get; set; }
        [JsonPropertyName("multiverseid")]
        public string MultiverseID { get; set; }
        public string ImageUrl { get; set; }
        public List<ForeignName> foreignNames { get; set; }
    }

    public class ForeignName
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public string Type { get; set; }
        public string Flavor { get; set; }
        public string ImageUrl { get; set; }
        public string Language { get; set; }
        public int? MultiverseID { get; set; }
    }
    public class CardApiResponse
    {
        public List<Card> Cards { get; set; }
        
    }


    public class CardLanguageConverter : JsonConverter<Card>
    {
        public override Card Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Create a dictionary to map language names to deserialized Card objects
            var languageToCardMap = new Dictionary<string, Card>();

            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;

                // Loop through the foreignNames array and extract language-specific information
                foreach (var foreignNameElement in root.GetProperty("foreignNames").EnumerateArray())
                {
                    string language = foreignNameElement.GetProperty("language").GetString();
                    var card = JsonSerializer.Deserialize<Card>(foreignNameElement.GetRawText());

                    if (!languageToCardMap.ContainsKey(language))
                    {
                        // Handle multiverseid conversion to integer or null
                        var multiverseidProperty = foreignNameElement.GetProperty("multiverseid");
                        if (multiverseidProperty.ValueKind == JsonValueKind.Number)
                        {
                            card.MultiverseID = multiverseidProperty.GetInt32().ToString();
                        }
                        else
                        {
                            card.MultiverseID = null; // or set to a default value
                        }

                        languageToCardMap[language] = card;
                    }
                }

            }

            // Here, you can return the desired language's Card object
            if (languageToCardMap.ContainsKey("English"))
            {
                return languageToCardMap["English"];
            }

            // Handle unsupported languages or return a default Card if needed
            return null;
        }

        public override void Write(Utf8JsonWriter writer, Card value, JsonSerializerOptions options)
        {
            // Implement write if necessary
        }
    }


}

