using System.Data;
using System;
using Dapper;
using System.Linq;
using PSW.ITT.Data.Entities;
using  PSW.ITT.Data.IRepositories;
using System.Collections.Generic;
using PSW.ITT.Data.DTO;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class LPCORegulationRepository : Repository<LPCORegulation>, ILPCORegulationRepository
    {
        #region public constructors

        public LPCORegulationRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[LPCORegulation]";
            PrimaryKeyName = "ID";
        }

        #endregion

        #region Public methods
        public LPCORegulation CheckIfRecordAlreadyExistInTheSystem(string hsCode, string productCode, string tradeTranTypeID,  int agencyID, string factor){
        var query = @"Select * FROM LPCORegulation l
                        JOIN ProductCode p ON p.HsCode = l.HsCode and p.HsCodeExt= l HsCodeExt and  ( p.tradeTranTypeID= 3 or p.tradeTranTypeID=l.tradeTranTypeID)
                        JOIN [ProductCodeAgencyLink] a ON a.ProductCodeID = p.ID AND a.AgencyID = l.AgencyID AND 
 SELECT [ProductCodeID] ,[AgencyID],[EffectiveFromDt] ,[EffectiveThruDt],[SHRD]..[agency].Code,[SHRD]..[agency].Name
                         FROM [ITT].[dbo].[ProductCodeAgencyLink]
                         Left Join [SHRD]..[agency] on [SHRD]..[agency].[ID] = [ProductCodeAgencyLink].[AgencyID]
                         WHERE (EffectiveFromDt <= GetDate() AND EffectiveThruDt >= GetDate()) 
                         or (EffectiveFromDt >= GetDate() AND EffectiveThruDt >= GetDate())";

            return _connection.Query<LPCORegulation>(
                    query,
                    transaction: _transaction
                   ).FirstOrDefault();
        }
        #endregion
    }
}
