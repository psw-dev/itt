using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using PSW.ITT.Data.Entities;
using  PSW.ITT.Data.IRepositories;
using PSW.ITT.Data.DTO;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class Ref_HS_CodesRepository : Repository<Ref_HS_Codes>, IRef_HS_CodesRepository
    {
		#region public constructors

        public Ref_HS_CodesRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[Ref_HS_Codes]";
			PrimaryKeyName = "HS_CODE";
        }

        
        #endregion

        #region Public methods

      
        public Ref_HS_Codes GetHsCodeList(string hsCode)
        {
             var query = @"SELECT *
                            FROM SHRD..Ref_HS_Codes c 
                            WHERE GETDATE() BETWEEN c.Effective_Date AND c.End_Date 
                            AND Hs_Code = @HSCODE";
                            
             return  _connection.Query<Ref_HS_Codes>(query, param: new { HSCODE = hsCode },
              transaction: _transaction
             ).FirstOrDefault();
        }  
        #endregion
    }
}
