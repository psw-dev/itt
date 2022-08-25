using PSW.ITT.Service.Command;
using System.Text.Json;

namespace PSW.ITT.Service.Strategies
{
    public class InvalidStrategy : Strategy
    {
        public InvalidStrategy(CommandRequest request) : base(request, "")
        {

        }

        public override CommandReply Execute()
        {
            //this.Command
            var json = "{}";

            return new CommandReply()
            {
                message = "Invalid Method",
                data = JsonDocument.Parse(json).RootElement,
                code = "404"
            };
        }
    }
}
