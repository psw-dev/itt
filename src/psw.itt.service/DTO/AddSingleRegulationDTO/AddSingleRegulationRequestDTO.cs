using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class AddSingleRegulationRequestDTO
    {
        [JsonPropertyName("data")]
        public dynamic Data { get; set; }

        [JsonPropertyName("tradeTranTypeID")]
        public short TradeTranTypeID { get; set; }

        [JsonPropertyName("agencyID")]
        public short AgencyID { get; set; }

        [JsonPropertyName("productCodeAgencyLinkID")]
        public long ProductCodeAgencyLinkID { get; set; }

    }
}
