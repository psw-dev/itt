using System.Text.Json.Serialization;

using System.Collections.Generic;

namespace PSW.ITT.Data.Entities
{
    public class AgencyList
    {
        [JsonPropertyName("agencyID")]
        public int Id { get; set; }

        [JsonPropertyName("agencyName")]
        public string Name { get; set; }

        [JsonPropertyName("requiredDocumentCode")]
        public string RequiredDocumentCode { get; set; }

        [JsonPropertyName("itemDescription")]
        public string ItemDescription { get; set; }
    }
}
