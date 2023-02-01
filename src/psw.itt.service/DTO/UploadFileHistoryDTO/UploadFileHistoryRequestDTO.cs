using System.Text.Json.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace PSW.ITT.Service.DTO
{
    public class UploadFileHistoryRequestDTO
    {
        [JsonPropertyName("agencyID")]
        public short AgencyID { get; set; } 

        [JsonPropertyName("event")]
        public bool Event { get; set; } 

        [JsonPropertyName("sheetType")]
        public List<int> SheetType { get; set; } 

    }
}
