using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class FetchActiveProductCodesListResponseDTO
    {
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

        [JsonPropertyName("effectiveFromDt")]
        public DateTime EffectiveFromDt { get; set; }

        [JsonPropertyName("effectiveThruDt")]
        public DateTime EffectiveThruDt { get; set; }
    }
}
