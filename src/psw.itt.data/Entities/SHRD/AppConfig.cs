/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System;
using System.Collections.Generic;


namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the AppConfig table in the database 
    /// </summary>
	public class AppConfig : Entity
    {
        #region Fields

        private string _key;
        private string _value;
        private string _appName;
        private string _dataType;
        private string _description;
        private DateTime _createdOn;
        private int _createdBy;
        private DateTime _updatedOn;
        private int _updatedBy;

        #endregion

        #region Properties

        public string Key { get { return _key; } set { _key = value; PrimaryKey = value; } }
        public string Value { get { return _value; } set { _value = value; } }
        public string AppName { get { return _appName; } set { _appName = value; } }
        public string DataType { get { return _dataType; } set { _dataType = value; } }
        public string Description { get { return _description; } set { _description = value; } }
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
                {"Key", Key},
                {"Value", Value},
                {"AppName", AppName},
                {"DataType", DataType},
                {"Description", Description},
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

