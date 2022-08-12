using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class GetFilesDetailRequestDTO
    {
        [JsonPropertyName("files")]
        public List<string> Files { get; set; }
    }
}