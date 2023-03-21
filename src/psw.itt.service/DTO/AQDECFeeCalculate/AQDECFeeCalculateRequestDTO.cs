using System.Collections.Generic;
using System.Text.Json.Serialization;
using PSW.ITT.Data.Entities;

namespace PSW.ITT.Service.DTO
{
    public class AQDECFeeCalculateRequestDTO
    { 
        [JsonPropertyName("hsCodeExt")]
        public string HsCodeExt { get; set; }

        [JsonPropertyName("agencyId")]
        public int AgencyId { get; set; }

        [JsonPropertyName("agencyUOMId")]
        public int AgencyUOMId { get; set; }

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("tradeTranTypeId")]
        public int TradeTranTypeID { get; set; }

        [JsonPropertyName("lpcoRegulation")]
        public LPCORegulation LPCORegulation { get; set; }


    }
}