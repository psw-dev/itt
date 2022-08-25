using System.Text.Json.Serialization;

namespace psw.itt.common
{
    public class UserRoleDTO 
    {
        [JsonPropertyName("userRoleId")]
        public int UserRoleID { get; set; }

        [JsonPropertyName("roleCode")]
        public string RoleCode { get; set; }
    }
}