using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class ColumnNamesForExcelRequestDTO
    {
        [JsonPropertyName("tradeTranTypeID")]
        public short TradeTranTypeID { get; set; }

        [JsonPropertyName("action")]
        public short ActionID { get; set; }
        
        [JsonPropertyName("agencyID")]
        public short AgencyID { get; set; }
        
        [JsonPropertyName("sheetType")]
        public short SheetType { get; set; }
    }
}
