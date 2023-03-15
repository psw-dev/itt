using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PSW.ITT.Common.Model
{
    public class SingleResponseModel<TModel>
    {
        #region Public Properties

        [JsonIgnore]
        public ErrorResponseModel Error { get; set; }

        [JsonIgnore]
        public bool IsError { get; set; }

        public TModel Model { get; set; }

        #endregion Public Properties
    }
}
