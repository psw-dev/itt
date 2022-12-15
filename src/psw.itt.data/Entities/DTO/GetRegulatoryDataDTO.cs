using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Data.DTO
{
    public class  GetRegulatoryDataDTO
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

        [JsonPropertyName("regulationJson")]
        public string RegulationJson { get; set; }
        [JsonPropertyName("effectiveFromDt")]
        public DateTime EffectiveFromDt { get; set; }
        [JsonPropertyName("effectiveThruDt")]
        public DateTime EffectiveThruDt { get; set; }

    }
}
