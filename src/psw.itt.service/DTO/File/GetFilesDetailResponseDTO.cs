using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class GetFilesDetailResponseDTO
    {
        [JsonPropertyName("files")]
        public List<FileDTO> Files { get; set; }
    }

    public class FileDTO
    {
        [JsonPropertyName("fileId")]
        public int Id { get; set; }

        [JsonPropertyName("fileName")]
        public string FileNameDisplay { get; set; }

        [JsonPropertyName("identification")]
        public string Identification { get; set; }

        [JsonPropertyName("encryptedFileID")]
        public string EncryptedFileID { get; set; }
    }
}