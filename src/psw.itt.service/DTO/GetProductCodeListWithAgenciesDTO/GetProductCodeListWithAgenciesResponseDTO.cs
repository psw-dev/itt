using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class GetProductCodeListWithAgenciesResponseDTO
    {
        [JsonPropertyName("productCodeID")]
        public long ProductCodeID { get; set; }
        [JsonPropertyName("agencyID")]
        public long AgencyID { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
