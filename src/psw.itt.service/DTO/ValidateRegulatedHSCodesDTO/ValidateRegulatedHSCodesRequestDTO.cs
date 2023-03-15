using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace PSW.ITT.Service.DTO
{
    public class ValidateRegulatedHSCodesRequestDTO
    {
        [JsonPropertyName("hsCodes")]
        public List<string> HSCodes { set; get; }
        
        [JsonPropertyName("agencyId")]
        public int AgencyID { set; get; }
        
        [JsonPropertyName("tradeTranTypeId")]
        public int TradeTranTypeId { set; get; }

        
    }
}