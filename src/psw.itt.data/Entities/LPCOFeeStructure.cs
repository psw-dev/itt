/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/

using System;
using System.Collections.Generic;

namespace PSW.ITT.Data.Entities
{
    /// <summary>
    /// This class represents the LPCOFeeStructure table in the database 
    /// </summary>
	public class LPCOFeeStructure : Entity
	{
		#region Fields
		
		private long _iD;
		private short _agencyID;
		private short _oGAItemCategoryID;
		private int? _unit_ID;
		private string _calculationBasis;
		private string _calculationSource;
		private int? _qtyRangeTo;
		private int? _qtyRangeFrom;
		private string _currencyCode;
		private decimal? _rate;
		private int? _factorValueID;
		private decimal? _minAmount;
		private decimal? _additionalAmount;
		private string _additionalAmountOn;
		private bool _isActive;
		private DateTime _createdOn;
		private int _createdBy;
		private DateTime _updatedOn;
		private int _updatedBy;

		#endregion

		#region Properties
		
		public long ID { get { return _iD; } set { _iD = value; PrimaryKey = value; }}
		public short AgencyID { get { return _agencyID; } set { _agencyID = value;  }}
		public short OGAItemCategoryID { get { return _oGAItemCategoryID; } set { _oGAItemCategoryID = value;  }}
		public int? Unit_ID { get { return _unit_ID; } set { _unit_ID = value;  }}
		public string CalculationBasis { get { return _calculationBasis; } set { _calculationBasis = value;  }}
		public string CalculationSource { get { return _calculationSource; } set { _calculationSource = value;  }}
		public int? QtyRangeTo { get { return _qtyRangeTo; } set { _qtyRangeTo = value;  }}
		public int? QtyRangeFrom { get { return _qtyRangeFrom; } set { _qtyRangeFrom = value;  }}
		public string CurrencyCode { get { return _currencyCode; } set { _currencyCode = value;  }}
		public decimal? Rate { get { return _rate; } set { _rate = value;  }}
		public int? FactorValueID { get { return _factorValueID; } set { _factorValueID = value;  }}
		public decimal? MinAmount { get { return _minAmount; } set { _minAmount = value;  }}
		public decimal? AdditionalAmount { get { return _additionalAmount; } set { _additionalAmount = value;  }}
		public string AdditionalAmountOn { get { return _additionalAmountOn; } set { _additionalAmountOn = value;  }}
		public bool IsActive { get { return _isActive; } set { _isActive = value;  }}
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
				{"AgencyID", AgencyID},
				{"OGAItemCategoryID", OGAItemCategoryID},
				{"Unit_ID", Unit_ID},
				{"CalculationBasis", CalculationBasis},
				{"CalculationSource", CalculationSource},
				{"QtyRangeTo", QtyRangeTo},
				{"QtyRangeFrom", QtyRangeFrom},
				{"CurrencyCode", CurrencyCode},
				{"Rate", Rate},
				{"FactorValueID", FactorValueID},
				{"MinAmount", MinAmount},
				{"AdditionalAmount", AdditionalAmount},
				{"AdditionalAmountOn", AdditionalAmountOn},
				{"IsActive", IsActive},
				{"CreatedOn", CreatedOn},
				{"CreatedBy", CreatedBy},
				{"UpdatedOn", UpdatedOn},
				{"UpdatedBy", UpdatedBy}
			};
        }
		 public override object GetInsertUpdateParams()
        {
            return new
            {
                ID
				,AgencyID
				,OGAItemCategoryID
				,Unit_ID
				,CalculationBasis
				,CalculationSource
				,QtyRangeTo
				,QtyRangeFrom
				,CurrencyCode
				,Rate
				,FactorValueID
				,MinAmount
				,AdditionalAmount
				,AdditionalAmountOn
				,IsActive
				,CreatedOn
				,CreatedBy
				,UpdatedOn
				,UpdatedBy
            };
        }


		#endregion

		#region Constructors
		 public LPCOFeeStructure()
        {
            TableName = "LPCOFeeStructure";
            PrimaryKeyName = "ID";
        }
		#endregion
	}
} 

