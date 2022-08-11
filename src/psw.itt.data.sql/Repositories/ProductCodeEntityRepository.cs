using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using psw.itt.data.Entities;
using psw.itt.data.IRepositories;
using SqlKata;

namespace psw.itt.data.sql.Repositories
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
            var query = new Query("ProductCode")
              .Where("EffectiveFromDt", "<=", DateTime.Now)
              .Where("EffectiveThruDt", ">=", DateTime.Now)
              .Select("*");

            var result = _sqlCompiler.Compile(query);
            var sql = result.Sql;
            var parameters = new DynamicParameters(result.NamedBindings);

            return _connection.Query<ProductCodeEntity>(sql, param: parameters, transaction: _transaction).ToList();
        }
        public List<ProductCodeEntity> GetOverlappingEffectiveFromProductCode(string hscode, string ProductCode, DateTime effectiveFromDt)
        {
            var query = new Query("ProductCode")
                .Where("HSCode", "=", hscode)
                .Where("ProductCode", "=", ProductCode)
                .Where("EffectiveFromDt", "<=", effectiveFromDt)
                .Where("EffectiveThruDt", ">=", effectiveFromDt)
                .Select("*");

            var result = _sqlCompiler.Compile(query);
            var sql = result.Sql;
            var parameters = new DynamicParameters(result.NamedBindings);

            return _connection.Query<ProductCodeEntity>(sql, param: parameters, transaction: _transaction).ToList();
        }
        #endregion
    }
}
