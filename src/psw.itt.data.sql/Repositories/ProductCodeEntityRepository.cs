using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.IRepositories;
using SqlKata;

namespace PSW.ITT.Data.Sql.Repositories
{
    public class ProductCodeEntityRepository : Repository<ProductCodeEntity>, IProductCodeEntityRepository
    {
        #region public constructors

        public ProductCodeEntityRepository(IDbConnection context) : base(context)
        {
            TableName = "[dbo].[ProductCode]";
            PrimaryKeyName = "ID";
        }

        #endregion

        #region Public methods
        public List<ProductCodeEntity> GetActiveProductCode()
        {
            try
            {
                var query = @"SELECT * FROM ProductCode WHERE EffectiveThruDt >= GETDATE()";

                return _connection.Query<ProductCodeEntity>(query,
                transaction: _transaction
                ).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<ProductCodeEntity> GetOverlappingProductCode(string hscode, string ProductCode, DateTime effectiveFromDt, DateTime effectiveThruDt)
        {
            var query = new Query("ProductCode")
                .Where("HSCode", "=", hscode)
                .Where("ProductCode", "=", ProductCode)
                .Where("EffectiveFromDt", ">", effectiveThruDt)
                .OrWhere("EffectiveThruDt", "<", effectiveFromDt)
                .Select("*");


            var result = _sqlCompiler.Compile(query);
            var sql = result.Sql;
            var parameters = new DynamicParameters(result.NamedBindings);

            return _connection.Query<ProductCodeEntity>(sql, param: parameters, transaction: _transaction).ToList();
        }
        #endregion
    }
}
