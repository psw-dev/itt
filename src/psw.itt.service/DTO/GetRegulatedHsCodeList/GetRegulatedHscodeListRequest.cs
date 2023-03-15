using System.Text.Json.Serialization;


namespace PSW.ITT.Service.DTO
{
    public class GetRegulatedHscodeListRequest
    {
        [JsonPropertyName("agencyId")]
        public int AgencyId { set; get; }

        [JsonPropertyName("chapter")]
        public string Chapter { set; get; }

        [JsonPropertyName("tradeTranTypeID")]
        public int TradeTranTypeID { set; get; }
    }
}