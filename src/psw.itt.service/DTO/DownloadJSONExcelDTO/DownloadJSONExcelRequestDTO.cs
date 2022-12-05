using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class DownloadJSONExcelRequestDTO
    {
        [JsonPropertyName("tradeTranTypeID")]
        public short TradeTranTypeID { get; set; }

        [JsonPropertyName("agencyID")]
        public short AgencyID { get; set; }
    }
}
