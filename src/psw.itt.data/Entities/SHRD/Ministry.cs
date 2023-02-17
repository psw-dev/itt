/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System.Collections.Generic;


namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the Ministry table in the database 
    /// </summary>
	public class Ministry : Entity
    {
        #region Fields

        private short _iD;
        private string _name;

        #endregion

        #region Properties

        public short ID { get { return _iD; } set { _iD = value; PrimaryKey = value; } }
        public string Name { get { return _name; } set { _name = value; } }

        #endregion

        #region Methods

        #endregion

        #region public Methods

        public override Dictionary<string, object> GetColumns()
        {
            return new Dictionary<string, object>
            {
                {"ID", ID},
                {"Name", Name}
            };
        }

        #endregion

        #region Constructors

        #endregion
    }
}

