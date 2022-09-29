using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class FetchActiveProductCodesListRequestDTO
    {
        [JsonPropertyName("userRole")]
        public string userRole { get; set; }

        [JsonPropertyName("agencyID")]
        public int? agencyID { get; set; }
    }
}
