using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

namespace PSW.ITT.Common.Model
{
    public class BaseErrorModel
    {
        #region Public Properties

        public string Code { get; set; }

        [JsonIgnore]
        public HttpStatusCode HttpCode { get; set; }

        public string Message { get; set; }

        #endregion Public Properties
    }

    public class ErrorDetails : BaseErrorModel
    {
        #region Public Properties

        public string Target { get; set; }

        #endregion Public Properties
    }

    public class ErrorModel : BaseErrorModel
    {
        #region Public Constructors

        public ErrorModel()
        {
            Details = new List<ErrorDetails>();
        }

        #endregion Public Constructors

        #region Public Properties

        [JsonPropertyName("target")]
        public string Category { get; set; }
        public List<ErrorDetails> Details { get; set; }

        #endregion Public Properties
    }
}
