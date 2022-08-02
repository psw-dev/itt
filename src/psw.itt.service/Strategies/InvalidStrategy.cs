using psw.itt.service.Command;
using System.Text.Json;

namespace psw.itt.service.Strategies
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
