using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class UploadConfigrationFileRequestDTO
    {

        [JsonPropertyName("filepath")]
        public string FilePath { get; set; }

        [JsonPropertyName("fileId")]
        public long FileID { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; }
        
        [JsonPropertyName("roleCode")]
        public string RoleCode { get; set; }

        [JsonPropertyName("agencyID")]
        public short AgencyID { get; set; }

        [JsonPropertyName("tradeTranTypeID")]
        public short TradeTranTypeID { get; set; }

        [JsonPropertyName("actionID")]
        public short ActionID { get; set; }

        [JsonPropertyName("fileType")]
        public short FileType { get; set; }

    }
}
