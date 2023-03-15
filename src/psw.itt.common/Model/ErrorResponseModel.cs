using PSW.ITT.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

namespace PSW.ITT.Common.Model
{
    public class ErrorResponseModel
    {
        #region Public Constructors

        public ErrorResponseModel()
        {
            InternalError = new ErrorModel();
        }

        #endregion Public Constructors

        #region Public Properties

        [JsonPropertyName("error")]
        public ErrorModel InternalError { get; set; }

        #endregion Public Properties

        #region Public Methods

        public ErrorResponseModel AddDetails(string key, string value)
        {
            ParseErrorDetails(key, value);
            return this;
        }

        public ErrorResponseModel AddDetails(Dictionary<string, string> data)
        {
            foreach (var key in data.Keys)
            {
                ParseErrorDetails(key, data[key]);
            }

            return this;
        }

        public ErrorResponseModel AddException(Exception ex)
        {
            ParseErrorDetails(ex.TargetSite?.Name,
                $"E999:Exception Message @ {ex.Message}, Inner Exception @ {ex.InnerException}, Stack Trace @ {ex.StackTrace}");

            return this;
        }

        public ErrorResponseModel Category(string errorCategory)
        {
            InternalError.Category = errorCategory;

            return this;
        }

        public ErrorResponseModel Code(string errorCode)
        {
            InternalError.Code = errorCode;

            return this;
        }

        public ErrorResponseModel Exception(Exception ex, bool throwException)
        {
            InternalError.Category = ErrorCategories.InternalServer;
            InternalError.Code = "9999";
            InternalError.Message = "An exception occurred while processing request.";
            if (throwException)
            {
                AddException(ex);
            }

            return this;
        }

        public ErrorResponseModel Exception(Exception ex)
        {
            InternalError.Category = ErrorCategories.InternalServer;
            InternalError.Code = "9999";
            InternalError.Message = "An exception occurred while processing request.";
            AddException(ex);

            return this;
        }

        public ErrorResponseModel FieldValidationError(Dictionary<string, string> errors)
        {
            InternalError.Category = ErrorCategories.Validation;
            InternalError.Code = "V999";
            InternalError.Message = "Validation errors occurred.";

            foreach (var key in errors.Keys)
            {
                ParseErrorDetails(key, errors[key]);
            }

            return this;
        }

        public ErrorResponseModel MapCategory(string httpStatusCode)
        {
            if (!string.IsNullOrWhiteSpace(httpStatusCode))
            {
                if ((HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), httpStatusCode) == HttpStatusCode.NotFound)
                {
                    InternalError.Category = ErrorCategories.NotFound;
                }
            }

            return this;
        }

        public ErrorResponseModel Message(string errorMessage)
        {
            InternalError.Message = errorMessage;

            return this;
        }

        public ErrorResponseModel PaginationError()
        {
            InternalError.Category = ErrorCategories.NotFound;
            InternalError.Code = "P999";
            InternalError.Message = "An error occurred while pagination.";

            return this;
        }

        public HttpStatusCode ToHttpStatusCode()
        {
            switch (InternalError.Category)
            {
                case ErrorCategories.BusinessRule:
                    return HttpStatusCode.Forbidden;

                case ErrorCategories.Validation:
                    return HttpStatusCode.BadRequest;

                case ErrorCategories.Authorization:
                    return HttpStatusCode.Unauthorized;

                case ErrorCategories.Identification:
                    return HttpStatusCode.NotFound;

                case ErrorCategories.InternalServer:
                    return HttpStatusCode.InternalServerError;

                case ErrorCategories.ServiceUnavailable:
                    return HttpStatusCode.ServiceUnavailable;

                case ErrorCategories.NotFound:
                    return HttpStatusCode.NotFound;

                default:
                    return HttpStatusCode.InternalServerError;
            }
        }

        #endregion Public Methods

        #region Private Methods

        private string ExtractMessage(string value)
        {
            var colonPosition = value.IndexOf(":");
            var messageLength = value.Length - colonPosition - 1;

            return value.Substring(colonPosition + 1, messageLength);
        }

        private void ParseErrorDetails(string key, string value)
        {
            //var applicationAbstraction = new Application_Abstraction();

            var defaultMessage = string.IsNullOrWhiteSpace(value)
                    ? value
                    : (value.Contains(":") ? ExtractMessage(value) : value);
            var responseCode = string.IsNullOrWhiteSpace(value) ? InternalError.Code : value?.Split(':').FirstOrDefault();

            //var internalRequest = new TPS.Infrastructure.Common.Services.Response.Types.GetResponseMessageDTO()
            //{
            //    InstitutionCode = "Default",
            //    ResponseCode = responseCode,
            //    LanguageCulture = string.Empty
            //};
            //var internalResponse = string.Empty;

            //try
            //{
            //    internalResponse = applicationAbstraction.GetResponseMessageByLanguage(internalRequest);
            //}
            //catch (Exception ex)
            //{
            //    internalResponse = string.Empty;
            //}

            InternalError.Details.Add(new ErrorDetails
            {
                Target = key,
                Code = responseCode,
                Message = defaultMessage
            });
        }

        #endregion Private Methods
    }
}

