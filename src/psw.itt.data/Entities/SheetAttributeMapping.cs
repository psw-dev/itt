/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System;
using System.Collections.Generic;
using System.Linq;


namespace psw.itt.data.Entities
{
    /// <summary>
    /// This class represents the SheetAttributeMapping table in the database 
    /// </summary>
	public class SheetAttributeMapping : Entity
	{
		#region Fields
		
		private int _iD;
		private string _nameLong;
		private string _nameShort;
		private short _index;
		private bool _isActive;
		private bool _isMandatory;
		private DateTime _createdOn;
		private int _createdBy;
		private DateTime _updatedOn;
		private int _updatedBy;

		#endregion

		#region Properties
		
		public int ID { get { return _iD; } set { _iD = value; PrimaryKey = value; }}
		public string NameLong { get { return _nameLong; } set { _nameLong = value;  }}
		public string NameShort { get { return _nameShort; } set { _nameShort = value;  }}
		public short Index { get { return _index; } set { _index = value;  }}
		public bool IsActive { get { return _isActive; } set { _isActive = value;  }}
		public bool IsMandatory { get { return _isMandatory; } set { _isMandatory = value;  }}
		public DateTime CreatedOn { get { return _createdOn; } set { _createdOn = value;  }}
		public int CreatedBy { get { return _createdBy; } set { _createdBy = value;  }}
		public DateTime UpdatedOn { get { return _updatedOn; } set { _updatedOn = value;  }}
		public int UpdatedBy { get { return _updatedBy; } set { _updatedBy = value;  }}

		#endregion

		#region Methods

		#endregion

		#region public Methods

		public override Dictionary<string, object> GetColumns()
        {
            return new Dictionary<string, object> 
			{
				{"ID", ID},
				{"NameLong", NameLong},
				{"NameShort", NameShort},
				{"Index", Index},
				{"IsActive", IsActive},
				{"IsMandatory", IsMandatory},
				{"CreatedOn", CreatedOn},
				{"CreatedBy", CreatedBy},
				{"UpdatedOn", UpdatedOn},
				{"UpdatedBy", UpdatedBy},
			};
        }
 public override object GetInsertUpdateParams()
        {
            return new
            {

                NameLong,
                NameShort,
                Index,
                IsActive,
				IsMandatory,
                CreatedBy,
                CreatedOn,
                UpdatedBy,
                UpdatedOn
            };
        }

		#endregion

		#region Constructors
		 public SheetAttributeMapping()
        {
            TableName = "SheetAttributeMapping";
            PrimaryKeyName = "ID";
        }
		#endregion
	}
} 

