using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class FetchLOVDataRequestDTO
    {
        [JsonPropertyName("tradeTranTypeID")]
        public short TradeTranTypeID { get; set; }

        [JsonPropertyName("agencyID")]
        public short AgencyID { get; set; }
    }
}
