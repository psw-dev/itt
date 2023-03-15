/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System;
using System.Collections.Generic;



namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the UoM table in the database 
    /// </summary>
	public class UoM : Entity
    {
        #region Fields

        private System.SByte _iD;
        private string _code;
        private string _name;
        private System.SByte _webocUoMID;
        private DateTime _createdOn;
        private int _createdBy;
        private DateTime _updatedOn;
        private int _updatedBy;

        #endregion

        #region Properties

        public System.SByte ID { get { return _iD; } set { _iD = value; PrimaryKey = value; } }
        public string Code { get { return _code; } set { _code = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public System.SByte WebocUoMID { get { return _webocUoMID; } set { _webocUoMID = value; } }
        public DateTime CreatedOn { get { return _createdOn; } set { _createdOn = value; } }
        public int CreatedBy { get { return _createdBy; } set { _createdBy = value; } }
        public DateTime UpdatedOn { get { return _updatedOn; } set { _updatedOn = value; } }
        public int UpdatedBy { get { return _updatedBy; } set { _updatedBy = value; } }

        #endregion

        #region Methods

        #endregion

        #region public Methods

        public override Dictionary<string, object> GetColumns()
        {
            return new Dictionary<string, object>
            {
                {"ID", ID},
                {"Code", Code},
                {"Name", Name},
                {"WebocUoMID", WebocUoMID},
                {"CreatedOn", CreatedOn},
                {"CreatedBy", CreatedBy},
                {"UpdatedOn", UpdatedOn},
                {"UpdatedBy", UpdatedBy}
            };
        }

        #endregion

        #region Constructors

        #endregion
    }
}

