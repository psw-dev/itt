using System;
using System.Collections.Generic;

namespace psw.itt.data.Entities
{
    /// <summary>
    /// This class represents the ProductCodeChapter table in the database 
    /// </summary>
	public class ProductCodeChapter : Entity
    {
        #region Fields

        private long _iD;
        private string _code;
        private string _name;
        private string _description;
        private int _createdBy;
        private DateTime _createdOn;

        #endregion

        #region Properties

        public long ID { get { return _iD; } set { _iD = value; PrimaryKey = value; } }
        public string Code { get { return _code; } set { _code = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public int CreatedBy { get { return _createdBy; } set { _createdBy = value; } }
        public DateTime CreatedOn { get { return _createdOn; } set { _createdOn = value; } }
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
                {"Description", Description},
                {"CreatedBy", CreatedBy},
                {"CreatedOn", CreatedOn}
            };
        }
        public override object GetInsertUpdateParams()
        {
            return new
            {
                Code,
                Name,
                Description,
                CreatedBy,
                CreatedOn
            };
        }

        #endregion

        #region Constructors
        public ProductCodeChapter()
        {
            TableName = "ProductCodeChapter";
            PrimaryKeyName = "ID";
        }
        #endregion
    }
}

