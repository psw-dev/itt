using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using PSW.ITT.Data.DTO;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class ProductRegulationRequirementRepository : Repository<ProductRegulationRequirement>, IProductRegulationRequirementRepository
    {
        #region public constructors

        public ProductRegulationRequirementRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[ProductRegulationRequirement]";
            PrimaryKeyName = "ID";
        }

        #endregion

        #region Public methods
        public List<GetRegulatoryDataDTO> GetRegulatoryDataByTradeTypeAndAgency(short TradeTranTypeID, short AgencyID)
        {
            //         var query = @"  SELECT R.[ID]
            //   ,[ProductCodeAgencyLinkID]
            //   ,[RegulationEffectiveFromDt]
            //   ,[RegulationEffectiveThruDt]
            //   ,[TradeTranTypeID]
            //   ,R.[AgencyID]
            //   ,[RegulationJson]
            //    FROM [ITT].[dbo].[ProductCodeAgencyLink] P
            //    INNER JOIN [ITT].[dbo].[LPCORegulation] R ON R.ProductCodeAgencyLinkID = P.ID
            //    WHERE R.AgencyID = @AGENCYID AND TradeTranTypeID = @TRADETRANTYPEID
            //      AND ((P.RegulationEffectiveFromDt <= GetDate() AND P.RegulationEffectiveThruDt >= GetDate())
            //        OR (P.RegulationEffectiveFromDt >= GetDate() AND P.RegulationEffectiveThruDt >= GetDate())) 
            //        order by id DESC ";
            var query = @"  SELECT R.[ID]
      ,[ProductCodeAgencyLinkID]
      ,[RegulationEffectiveFromDt]
      ,[RegulationEffectiveThruDt]
      ,R.[EffectiveFromDt]
      ,R.[EffectiveThruDt]
      ,R.[TradeTranTypeID]
      ,R.[AgencyID]
      ,[RegulationJson]
      ,R.[HsCode]
      ,R.[HsCodeExt]
      ,[Factor]
      ,[Description]
       FROM [ITT].[dbo].[ProductCodeAgencyLink] P
       INNER JOIN [ITT].[dbo].[LPCORegulation] R ON R.ProductCodeAgencyLinkID = P.ID
       INNER JOIN [ITT].[dbo].[ProductCode] PC ON P.ProductCodeID = PC.ID
       WHERE R.AgencyID = @AGENCYID AND R.TradeTranTypeID = @TRADETRANTYPEID
         AND ((P.RegulationEffectiveFromDt <= GetDate() AND P.RegulationEffectiveThruDt >= GetDate())
           OR (P.RegulationEffectiveFromDt >= GetDate() AND P.RegulationEffectiveThruDt >= GetDate())) 
           AND ((R.EffectiveFromDt <= GetDate() AND R.EffectiveThruDt >= GetDate())
           OR (R.EffectiveFromDt >= GetDate() AND R.EffectiveThruDt >= GetDate())) 
           order by id DESC ";

            return _connection.Query<GetRegulatoryDataDTO>(
                    query, param: new { TRADETRANTYPEID = TradeTranTypeID, AGENCYID = AgencyID },
                    transaction: _transaction
                   ).ToList();
        }

        #endregion
    }
}
