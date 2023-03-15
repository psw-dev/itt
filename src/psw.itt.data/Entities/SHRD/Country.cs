/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System.Collections.Generic;


namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the Country table in the database 
    /// </summary>
	public class Country : Entity
    {
        #region Fields

        private string _code;
        private string _codeA3;
        private string _name;
        private short? _webocCountryId;

        #endregion

        #region Properties

        public string Code { get { return _code; } set { _code = value; PrimaryKey = value; } }
        public string CodeA3 { get { return _codeA3; } set { _codeA3 = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public short? WebocCountryId { get { return _webocCountryId; } set { _webocCountryId = value; } }

        #endregion

        #region Methods

        #endregion

        #region public Methods

        public override Dictionary<string, object> GetColumns()
        {
            return new Dictionary<string, object>
            {
                {"Code", Code},
                {"CodeA3", CodeA3},
                {"Name", Name},
                {"WebocCountryId", WebocCountryId}
            };
        }

        #endregion

        #region Constructors

        #endregion
    }
}

