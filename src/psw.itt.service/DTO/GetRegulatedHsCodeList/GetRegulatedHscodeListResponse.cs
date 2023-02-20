using PSW.ITT.Data.Entities;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class GetRegulatedHscodeListResponse
    {
        [JsonPropertyName("regulatedHsCodeList")]
        public List<ViewRegulatedHsCode> RegulatedHsCodeList { set; get; }
    }
}