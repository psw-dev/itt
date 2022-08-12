using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class SendInboxMessageRequestDTO
    {
        [JsonPropertyName("fromUserRoleId")]
        public int FromUserRoleId { get; set; }

        [JsonPropertyName("toUserRoleIds")]
        public List<int> ToUserRoleIds { get; set; }

        [JsonPropertyName("inboxRequestTypeId")]
        public byte InboxRequestTypeId { get; set; }

        [JsonPropertyName("placeholders")]
        public Dictionary<string, string> Placeholders { get; set; }
    }
}
