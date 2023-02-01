using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class UploadFileRequestDTO
    {

        [JsonPropertyName("filepath")]
        public string FilePath { get; set; }

        [JsonPropertyName("fileId")]
        public long FileID { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; }
        
        [JsonPropertyName("roleCode")]
        public string RoleCode { get; set; }
        
        [JsonPropertyName("fileType")]
        public int FileType { get; set; }
    }
}
