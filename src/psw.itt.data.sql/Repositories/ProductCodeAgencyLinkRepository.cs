using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using PSW.ITT.Data.DTO;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;
using SqlKata;


namespace PSW.ITT.Data.Sql.Repositories
{
    public class ProductCodeAgencyLinkRepository : Repository<ProductCodeAgencyLink>, IProductCodeAgencyLinkRepository
    {
        #region public constructors

        public ProductCodeAgencyLinkRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[ProductCodeAgencyLink]";
            PrimaryKeyName = "ID";
        }

        #endregion

        #region Public methods
        public List<GetProductCodeListWithAgenciesResponseDTO> GetProductCodeIDWithOGA()
        {
            var query = @"SELECT [ProductCodeID] ,[AgencyID],[EffectiveFromDt] ,[EffectiveThruDt],[SHRD]..[agency].Code,[SHRD]..[agency].Name
                         FROM [ITT].[dbo].[ProductCodeAgencyLink]
                         Left Join [SHRD]..[agency] on [SHRD]..[agency].[ID] = [ProductCodeAgencyLink].[AgencyID]
                         WHERE (EffectiveFromDt <= GetDate() AND EffectiveThruDt >= GetDate()) 
                         or (EffectiveFromDt >= GetDate() AND EffectiveThruDt >= GetDate())";

            return _connection.Query<GetProductCodeListWithAgenciesResponseDTO>(
                    query,
                    transaction: _transaction
                   ).ToList();
        }
        #endregion
    }
}
