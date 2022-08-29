using System.Text.Json.Serialization;

namespace PSW.ITT.Common
{
    public class UserRoleDTO 
    {
        [JsonPropertyName("userRoleId")]
        public int UserRoleID { get; set; }

        [JsonPropertyName("roleCode")]
        public string RoleCode { get; set; }
    }
}