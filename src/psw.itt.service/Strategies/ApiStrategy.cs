using System.Linq;
using System.Text.Json;
using FluentValidation;
using psw.itt.common.Pagination;
using psw.itt.service.Command;

namespace psw.itt.service.Strategies
{
    /// <summary>
    /// Generic API strategy written specifically for an API call
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class ApiStrategy<T1, T2> : Strategy
    {
        /// <summary>
        /// API request object
        /// </summary>
        public T1 RequestDTO { get; set; }

        /// <summary>
        /// API response object
        /// </summary>
        public T2 ResponseDTO { get; set; }

        /// <summary>
        /// instance of AbstractValidator used to validate request object
        /// </summary>
        public AbstractValidator<T1> Validator { get; set; }

        /// <summary>
        /// validates request object, sets ValidationMessage and returns validation result as boolean
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            if (Validator != null)
            {
                string message = "validated";
                var results = Validator.Validate(RequestDTO);
                this.IsValidated = results.IsValid;

                if (!results.IsValid)
                {
                    var errors = results.Errors.Select(x => x.ErrorMessage).ToList();
                    message = errors.Aggregate((i, j) => i + "\n" + j);
                }

                this.ValidationMessage = message;

                return this.IsValidated;
            }

            IsValidated = true;
            return this.IsValidated;
        }


        public ServerPaginationModel Pagination { get; set; }

        /// <summary>
        /// Strategy name
        /// </summary>
        public string StrategyName { get; set; }
        public string MethodID { get; set; }

        public ApiStrategy(CommandRequest request) : base(request)
        {
            this.Command = request;
            this.IsValidated = false;
            this.StrategyName = GetType().Name;
            this.MethodID = request.methodId;
            this.Reply = new CommandReply();
            // Get Json Data From Command
            var jsonString = this.Command.data.GetRawText();
            // Deserialize Json to DTO
            RequestDTO = JsonSerializer.Deserialize<T1>(jsonString);
            Pagination = request.pagination;
        }

        public CommandReply OKReply(string message = "success")
        {
            Reply.code = "200"; // OK
            Reply.message = message;
            Reply.data = JsonDocument.Parse(JsonSerializer.Serialize(ResponseDTO)).RootElement;
            Reply.pagination = Pagination;
            return Reply;
        }

        public CommandReply BadRequestReply(string message, System.Exception exception, string shortDescription, string validationMessage)
        {
            Reply.code = "400"; // Bad Request | Error
            Reply.message = message;
            Reply.shortDescription = shortDescription;
            Reply.fullDescription = validationMessage;
            Reply.exception = exception.ToString();
            Reply.data = ResponseDTO != null
                ? JsonDocument.Parse(JsonSerializer.Serialize(ResponseDTO)).RootElement
                : JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(new { })).RootElement;
            return Reply;
        }

        public override CommandReply BadRequestReply(string message)
        {
            Reply.code = "400"; // Bad Request | Error
            Reply.message = message;
            Reply.exception = null;
            Reply.data = ResponseDTO != null
                ? JsonDocument.Parse(JsonSerializer.Serialize(ResponseDTO)).RootElement
                : JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(new { })).RootElement;
            return Reply;
        }

        public CommandReply NotFoundReply(string message = "Record Not Found")
        {
            Reply.code = "404";
            Reply.message = message;
            Reply.pagination = Pagination;
            return Reply;
        }

        public CommandReply InternalServerErrorReply(System.Exception exception, string message = "Internal Server Error")
        {
            Reply.code = "500"; // Internal Server Error | Error
            Reply.message = message;
            Reply.exception = exception.ToString();
            Reply.data = JsonDocument.Parse(JsonSerializer.SerializeToUtf8Bytes(new { })).RootElement;
            return Reply;
        }

        public string GetRequestValidationMessage()
        {
            string message = "validated";
            var results = Validator.Validate(RequestDTO);
            if (results.IsValid == false)
            {
                var errors = results.Errors.Select(x => x.ErrorMessage).ToList();
                message = errors.Aggregate((i, j) => i + "\n" + j);
            }

            ValidationMessage = message;

            return message;
        }

    }
}
