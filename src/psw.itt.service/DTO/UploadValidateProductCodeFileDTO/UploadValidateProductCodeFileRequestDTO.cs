using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class UploadValidateProductCodeFileRequestDTO
    {

        [JsonPropertyName("filepath")]
        public string FilePath { get; set; }

        [JsonPropertyName("fileId")]
        public long FileID { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; }
        
        [JsonPropertyName("roleCode")]
        public string RoleCode { get; set; }

        [JsonPropertyName("actionID")]
        public short ActionID { get; set; }

        [JsonPropertyName("agencyId")]
        public int AgencyId { get; set; }

        [JsonPropertyName("fileType")]
        public int FileType { get; set; }

    }
}
