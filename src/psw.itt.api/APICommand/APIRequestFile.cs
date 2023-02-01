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
        public string filePath { get; set; }
        public string fileName { get; set; }
        public long fileId { get; set; }
        public long agencyID { get; set; }
        public long tradeTranTypeID { get; set; }
        public long fileType { get; set; }
        public long actionID { get; set; }
        public string roleCode { get; set; }
    }

    public class Data
    {
        public string filepath { get; set; }
        public string fileName { get; set; }
        public long fileId { get; set; }
        public string roleCode { get; set; }
        public long agencyID { get; set; }
        public long tradeTranTypeID { get; set; }
        public long fileType { get; set; }
        public long actionID { get; set; }

    }
}