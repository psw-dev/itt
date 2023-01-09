using System.Collections.Generic;
using System.Text.Json.Serialization;
using PSW.ITT.Data.DTO;

namespace PSW.ITT.Service.DTO
{
    public class DownloadProductCodeExcelResponseDTO
    {
        [JsonPropertyName("gridColumns")]
        public List<GridColumns> GridColumns { get; set; }

        [JsonPropertyName("data")]
        public List<GetProductExcelDataDTO> Data { get; set; }

    }
}
