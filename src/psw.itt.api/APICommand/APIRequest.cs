using psw.itt.common.Pagination;
using System.Text.Json;

namespace psw.itt.api.APICommand
{
    public class APIRequest
    {
        public string methodId { get; set; }
        public JsonElement data { get; set; }
        public string signature { get; set; }
        public ServerPaginationModel pagination { get; set; }

    }
}