using System;
using System.Collections.Generic;


namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the Ref_HS_Codes table in the database 
    /// </summary>
	public class Ref_HS_Codes : Entity
	{
		#region Fields
		
		private int _hS_CODE_ID;
		private string _hS_CODE;
		private int? _serial;
		private DateTime? _effective_Date;
		private DateTime _end_Date;
		private string _description;
		private string _uOM_Code;
		private string _override;
		private DateTime? _entry_Date;
		private int? _user_Entered;
		private int? _user_Updated;
		private DateTime? _update_Date;
		private string _terminal_ID;
		private string _transaction_Type;
		private short? _bonding_Period;
		private bool? _is_Retail_Price;
		private bool? _is_Minimum_Value_Required;
		private bool? _is_Maximum_Value_Required;
		private bool? _is_Retail_Price_Mandatory;
		private string _uOM_Statistical_Purpose;
		private bool?  _isGSTRequired;

		#endregion

		#region Properties
		
		public int HS_CODE_ID { get { return _hS_CODE_ID; } set { _hS_CODE_ID = value;  }}
		public string HS_CODE { get { return _hS_CODE; } set { _hS_CODE = value; PrimaryKey = value; }}
		public int? Serial { get { return _serial; } set { _serial = value;  }}
		public DateTime? Effective_Date { get { return _effective_Date; } set { _effective_Date = value;  }}
		public DateTime End_Date { get { return _end_Date; } set { _end_Date = value; PrimaryKey = value; }}
		public string Description { get { return _description; } set { _description = value;  }}
		public string UOM_Code { get { return _uOM_Code; } set { _uOM_Code = value;  }}
		public string Override { get { return _override; } set { _override = value;  }}
		public DateTime? Entry_Date { get { return _entry_Date; } set { _entry_Date = value;  }}
		public int? User_Entered { get { return _user_Entered; } set { _user_Entered = value;  }}
		public int? User_Updated { get { return _user_Updated; } set { _user_Updated = value;  }}
		public DateTime? Update_Date { get { return _update_Date; } set { _update_Date = value;  }}
		public string Terminal_ID { get { return _terminal_ID; } set { _terminal_ID = value;  }}
		public string Transaction_Type { get { return _transaction_Type; } set { _transaction_Type = value;  }}
		public short? Bonding_Period { get { return _bonding_Period; } set { _bonding_Period = value;  }}
		public bool? Is_Retail_Price { get { return _is_Retail_Price; } set { _is_Retail_Price = value;  }}
		public bool? Is_Minimum_Value_Required { get { return _is_Minimum_Value_Required; } set { _is_Minimum_Value_Required = value;  }}
		public bool? Is_Maximum_Value_Required { get { return _is_Maximum_Value_Required; } set { _is_Maximum_Value_Required = value;  }}
		public bool? Is_Retail_Price_Mandatory { get { return _is_Retail_Price_Mandatory; } set { _is_Retail_Price_Mandatory = value;  }}
		public string UOM_Statistical_Purpose { get { return _uOM_Statistical_Purpose; } set { _uOM_Statistical_Purpose = value;  }}
		public bool? IsGSTRequired { get { return _isGSTRequired; } set { _isGSTRequired = value;  }}

		#endregion

		#region Methods

		#endregion

		#region public Methods

		public override Dictionary<string, object> GetColumns()
        {
            return new Dictionary<string, object> 
			{
				{"HS_CODE_ID", HS_CODE_ID},
				{"HS_CODE", HS_CODE},
				{"Serial", Serial},
				{"Effective_Date", Effective_Date},
				{"End_Date", End_Date},
				{"Description", Description},
				{"UOM_Code", UOM_Code},
				{"Override", Override},
				{"Entry_Date", Entry_Date},
				{"User_Entered", User_Entered},
				{"User_Updated", User_Updated},
				{"Update_Date", Update_Date},
				{"Terminal_ID", Terminal_ID},
				{"Transaction_Type", Transaction_Type},
				{"Bonding_Period", Bonding_Period},
				{"Is_Retail_Price", Is_Retail_Price},
				{"Is_Minimum_Value_Required", Is_Minimum_Value_Required},
				{"Is_Maximum_Value_Required", Is_Maximum_Value_Required},
				{"Is_Retail_Price_Mandatory", Is_Retail_Price_Mandatory},
				{"UOM_Statistical_Purpose", UOM_Statistical_Purpose},
				{"IsGSTRequired", IsGSTRequired}
			};
        }

		#endregion

		#region Constructors
		
		#endregion
	}
} 

