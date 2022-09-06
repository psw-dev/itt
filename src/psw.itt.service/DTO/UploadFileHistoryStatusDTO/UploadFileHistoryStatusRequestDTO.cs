using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class UploadFileHistoryStatusRequestDTO
    {
        [JsonPropertyName("id")]
        public long ID { get; set; }

        [JsonPropertyName("statusId")]
        public short StatusID { get; set; }

        [JsonPropertyName("roleCode")]
        public string RoleCode { get; set; }

    }
}
