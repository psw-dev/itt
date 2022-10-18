using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class UploadFileHistoryRequestDTO
    {
      [JsonPropertyName("agencyID")]
        public short AgencyID { get; set; } 

    }
}
