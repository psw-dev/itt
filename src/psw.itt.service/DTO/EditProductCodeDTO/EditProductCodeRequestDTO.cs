using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class EditProductCodeRequestDTO
    {
        [JsonPropertyName("hSCode")]
        public string HSCode { get; set; }

        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }

        [JsonPropertyName("effectiveFromDt")]
        public DateTime EffectiveFromDt { get; set; }

        [JsonPropertyName("effectiveThruDt")]
        public DateTime EffectiveThruDt { get; set; }

    }
}
