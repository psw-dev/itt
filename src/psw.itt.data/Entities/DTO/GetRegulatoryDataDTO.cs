using System.Text.Json.Serialization;

namespace PSW.ITT.Data.DTO
{
    public class GetRegulatoryDataDTO
    {
        [JsonPropertyName("id")]
        public long ID { get; set; }
        [JsonPropertyName("productCodeAgencyLinkID")]
        public long ProductCodeAgencyLinkID { get; set; }
        [JsonPropertyName("lPCORegulationID")]
        public long LPCORegulationID { get; set; }
        [JsonPropertyName("lPCOFeeStructureID")]
        public long LPCOFeeStructureID { get; set; }

        [JsonPropertyName("regulationJson")]
        public string RegulationJson { get; set; }

    }
}
