using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class UserInformationRequestDTO
    {
        [JsonPropertyName("userRoleID")]
        public int? UserRoleID { get; set; }
    }
}