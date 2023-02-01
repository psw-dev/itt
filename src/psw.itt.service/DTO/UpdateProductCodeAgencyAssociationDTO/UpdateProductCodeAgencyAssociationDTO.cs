using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class UpdateProductCodeAgencyAssociationRequestDTO
    {
        [JsonPropertyName("isAdd")]
        public bool isAdd { get; set; }

        [JsonPropertyName("productCodeID")]
        public long ProductCodeID { get; set; }
        [JsonPropertyName("agencyID")]
        public short AgencyID { get; set; }
    }
}
