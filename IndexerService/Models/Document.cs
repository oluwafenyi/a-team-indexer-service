using Newtonsoft.Json;

namespace IndexerService.Models
{
    public class Document
    {
        [JsonRequired]
        [JsonProperty("id")]
        public uint Id { get; set; }
        
        [JsonRequired]
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}