using PSW.ITT.Common.Pagination;
using System.Text.Json;

namespace PSW.ITT.Api.APICommand
{
    public class APIRequest
    {
        public string methodId { get; set; }
        public JsonElement data { get; set; }
        public string signature { get; set; }
        public ServerPaginationModel pagination { get; set; }

    }
}