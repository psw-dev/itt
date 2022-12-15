using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class GetRegulatoryDataResponseDTO
    {
        [JsonPropertyName("id")]
        public long ID { get; set; }

        [JsonPropertyName("productCodeAgencyLinkID")]
        public long ProductCodeAgencyLinkID { get; set; }

        [JsonPropertyName("hsCode")]
        public string HSCode { get; set; }
        [JsonPropertyName("hsCodeExt")]
        public string HSCodeExt { get; set; }
        [JsonPropertyName("factor")]
        public string Factor { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("data")]
        public dynamic Data { get; set; }

        [JsonPropertyName("effectiveFromDt")]
        public string EffectiveFromDt { get; set; }
        [JsonPropertyName("effectiveThruDt")]
        public string EffectiveThruDt { get; set; }

    }
}
