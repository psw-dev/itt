using PSW.ITT.Data.Entities;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class GetRegulatedHSCodeExtListResponse
    {
        [JsonPropertyName("regulatedHsCodeExtList")]
        public List<ViewRegulatedHsCodeExt> RegulatedHsCodeExtList { set; get; }
    }
}