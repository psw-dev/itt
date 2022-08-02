using System.Text.Json;
using psw.itt.common.Pagination;

namespace psw.itt.service.Command
{
    public class CommandReply
    {
        public JsonElement data { get; set; }
        public string exception { set; get; }
        public string shortDescription { get; set; }
        public string fullDescription { get; set; }
        public string code { get; set; }
        public string message { set; get; }
        public ServerPaginationModel pagination { get; set; }
        public CommandReply()
        {
            data = JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(new { })).RootElement;
        }
    }
}