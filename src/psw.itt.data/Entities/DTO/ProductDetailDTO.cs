using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Data.DTO
{
    public class ProductDetail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("productCode")]
        public string ProductCode { get; set; }

        [JsonPropertyName("itemDescription")]
        public string ItemDescription { get; set; }

    }
}
