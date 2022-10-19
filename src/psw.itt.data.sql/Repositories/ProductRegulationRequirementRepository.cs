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
            var query = @"  SELECT P.[ID]
      ,[ProductCodeAgencyLinkID]
      ,[LPCORegulationID]
      ,[LPCOFeeStructureID]
      ,[EffectiveFromDt]
      ,[EffectiveThruDt]
      ,[TradeTranTypeID]
      ,[AgencyID]
      ,[RegulationJson]
       FROM [ITT].[dbo].[ProductRegulationRequirement] P
       INNER JOIN [ITT].[dbo].[LPCORegulation] R ON R.ID = P.LPCORegulationID
       WHERE AgencyID = @AGENCYID AND TradeTranTypeID = @TRADETRANTYPEID
         AND ((P.EffectiveFromDt <= GetDate() AND P.EffectiveThruDt >= GetDate())
           OR (P.EffectiveFromDt >= GetDate() AND P.EffectiveThruDt >= GetDate())) ";

            return _connection.Query<GetRegulatoryDataDTO>(
                    query, param: new { TRADETRANTYPEID = TradeTranTypeID, AGENCYID = AgencyID },
                    transaction: _transaction
                   ).ToList();
        }

        #endregion
    }
}
