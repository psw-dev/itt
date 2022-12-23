using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class FetchActiveProductCodesListResponseDTO
    {
        [JsonPropertyName("serialId")]
        public long? SerialID { get; set; }

        [JsonPropertyName("id")]
        public long ID { get; set; }

        [JsonPropertyName("hSCode")]
        public string HSCode { get; set; }

        [JsonPropertyName("hSCodeExt")]
        public string HSCodeExt { get; set; }

        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }

        [JsonPropertyName("productCodeChapterID")]
        public short ProductCodeChapterID { get; set; }

        [JsonPropertyName("chapterCode")]
        public string ChapterCode { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("tradeType")]
        public string TradeType { get; set; }

        [JsonPropertyName("effectiveFromDt")]
        public string EffectiveFromDt { get; set; }

        [JsonPropertyName("effectiveThruDt")]
        public string EffectiveThruDt { get; set; }
        [JsonPropertyName("isActive")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("regulated")]
        public string Regulated { get; set; }
    }
}
