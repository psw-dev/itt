using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Data.DTO
{
    public class GetProductExcelDataDTO
    {
        [JsonPropertyName("chapter")]
        public string ChapterCode { get; set; }

        [JsonPropertyName("hsCode")]
        public string HSCode { get; set; }
        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("agencies")]
        public string Agencies { get; set; }
        [JsonPropertyName("tradeType")]
        public short TradeTranTypeID { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }

    }
}
