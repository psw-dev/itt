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
        public LPCORegulation CheckIfRecordAlreadyExistInTheSystem(string hsCode, string productCode, short tradeTranTypeID,  int agencyID, string factor){ //, DateTime EffectiveFromDt, DateTime EffectiveThruDt
        var query = @"Select l.* FROM LPCORegulation l
                        JOIN ProductCode p ON p.HsCode = l.HsCode and p.HsCodeExt= l.HsCodeExt and  ( p.tradeTranTypeID = 4 or p.tradeTranTypeID=l.tradeTranTypeID)
                        JOIN [ProductCodeAgencyLink] a ON a.ProductCodeID = p.ID AND a.AgencyID = l.AgencyID 
                        WHERE ((a.EffectiveFromDt <= GetDate() AND a.EffectiveThruDt >= GetDate()) 
                        OR (a.EffectiveFromDt >= GetDate() AND a.EffectiveThruDt >= GetDate()))
                        AND (l.EffectiveFromDt <= GetDate()  AND l.EffectiveThruDt >= GetDate())
                        AND l.HsCode = @HSCODE AND l.HsCodeExt = @PRODUCTCODE AND l.TradeTranTypeID = @TRADETRANTYPEID AND l.AgencyID = @AGENCYID AND LOWER(l.Factor) = @FACTOR";
                       //AND a.SoftDelete = 0 //(@EFFECTIVEFROMDATE >= l.EffectiveFromDt AND @EFFECTIVETHRUDATE <= l.EffectiveThruDt)
            return _connection.Query<LPCORegulation>(
                    query , param: new {HSCODE = hsCode, @PRODUCTCODE = productCode, @TRADETRANTYPEID = tradeTranTypeID, @AGENCYID = agencyID,@FACTOR = factor}, //, @EFFECTIVEFROMDATE = EffectiveFromDt, @EFFECTIVETHRUDATE = EffectiveThruDt
                    transaction: _transaction
                   ).FirstOrDefault();
        }
        #endregion
    }
}
