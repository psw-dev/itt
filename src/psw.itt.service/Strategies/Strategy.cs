using System.Net;
using AutoMapper;
using PSW.ITT.Service.Command;
using Serilog;

namespace PSW.ITT.Service.Strategies
{
    public class Strategy
    {
        public IMapper Mapper { get; protected set; }
        public CommandReply Reply { get; protected set; }
        public bool IsValidated { get; protected set; }
        protected string StrategyName => GetType().Name;
        public string MethodName { get; protected set; }
        protected string MethodId { get; }

        public CommandRequest Command { get; set; }
        public Strategy(CommandRequest request, string methodId)
        {
            Command = request;
            MethodId = methodId;
            IsValidated = false;
        }

        public Strategy(CommandRequest request)
        {
            Command = request;
            MethodId = request.methodId;
            IsValidated = false;
        }

        public virtual CommandReply Execute()
        {
            return null;
        }

        public string ValidationMessage { get; set; }
        public virtual bool Validate()
        {
            return this.IsValidated;
        }

        public CommandReply NotFoundReply(string message, string methodName)
        {
            Reply.code = ((int)HttpStatusCode.NotFound).ToString();  //"404";
            Reply.message = message;

            Log.Error("[{0}.{1}] {@Reply}", GetType().Name, methodName, Reply);
            return Reply;
        }

        public virtual CommandReply BadRequestReply(string message)
        {
            Reply.code = "400"; // Bad Request | Error
            Reply.message = message;
            Reply.exception = null;
            return Reply;
        }
    }
}
