using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class UpdateProductCodeAgencyStatusRequestDTO
    {
        [JsonPropertyName("status")]
        public bool status { get; set; }

        [JsonPropertyName("ID")]
        public long ID { get; set; }
    }
}
