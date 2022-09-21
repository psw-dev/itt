using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class UploadSingleProductCodeRequestDTO
    {
        [JsonPropertyName("hSCode")]
        public string HSCode { get; set; }

        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("effectiveFromDt")]
        public DateTime EffectiveFromDt { get; set; }

        [JsonPropertyName("effectiveThruDt")]
        public DateTime? EffectiveThruDt { get; set; }

        [JsonPropertyName("tradeType")]
        public string TradeType { get; set; }

    }
}
