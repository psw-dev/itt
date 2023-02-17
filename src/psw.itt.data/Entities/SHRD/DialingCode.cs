/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System.Collections.Generic;


namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the DialingCode table in the database 
    /// </summary>
	public class DialingCode : Entity
    {
        #region Fields

        private short _iD;
        private string _countryCode;
        private string _dialCode;

        #endregion

        #region Properties

        public short ID { get { return _iD; } set { _iD = value; PrimaryKey = value; } }
        public string CountryCode { get { return _countryCode; } set { _countryCode = value; } }
        public string DialCode { get { return _dialCode; } set { _dialCode = value; } }

        #endregion

        #region Methods

        #endregion

        #region public Methods

        public override Dictionary<string, object> GetColumns()
        {
            return new Dictionary<string, object>
            {
                {"ID", ID},
                {"CountryCode", CountryCode},
                {"DialCode", DialCode}
            };
        }

        #endregion

        #region Constructors

        #endregion
    }
}

