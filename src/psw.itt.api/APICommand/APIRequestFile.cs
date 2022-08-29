using System.Text.Json;
using Microsoft.AspNetCore.Http;
using PSW.ITT.Common.Pagination;

namespace PSW.ITT.Api.APICommand
{
    public class APIRequestFile
    {
        public string methodId { get; set; }
        public JsonElement data { get; set; }
        public string signature { get; set; }
        public ServerPaginationModel pagination { get; set; }
        public IFormFile file { get; set; }
        public string FilePath { get; set; }
        public string fileName { get; set; }
        public long fileId { get; set; }
        public string roleCode { get; set; }
    }

    public class Data
    {
        public string filepath { get; set; }
        public string fileName { get; set; }
        public long fileId { get; set; }
        public string roleCode { get; set; }
    }
}