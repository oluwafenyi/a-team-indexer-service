using System;
using Newtonsoft.Json;

namespace IndexerService.Models
{
    public class Document : IComparable<Document>
    {
        [JsonRequired]
        [JsonProperty("id")]
        public uint Id { get; set; }
        
        [JsonRequired]
        [JsonProperty("url")]
        public string Url { get; set; }

        public int CompareTo(Document other)
        {
            if (Id < other.Id) return -1;
            if (Id == other.Id) return 0;
            return 1;
        }
    }
}