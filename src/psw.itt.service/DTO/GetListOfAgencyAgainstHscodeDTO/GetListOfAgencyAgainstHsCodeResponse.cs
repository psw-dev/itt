using PSW.ITT.Data.Entities;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PSW.ITT.Service.DTO
{
    public class GetListOfAgencyAgainstHsCodeResponse
    {
        [JsonPropertyName("agencyDataList")]
        public List<AgencyList> AgencyList { get; set; }
    }
}