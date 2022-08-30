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
            var query = new Query("ProductCode")
              .WhereRaw("(EffectiveFromDt <= GetDate() AND EffectiveThruDt >= GetDate())")
              .OrWhereRaw("(EffectiveFromDt >= GetDate() AND EffectiveThruDt >= GetDate())")
              .SelectRaw("ROW_NUMBER() OVER(Order By(Select 1)), *")
              .OrderBy("EffectiveThruDt");

            var result = _sqlCompiler.Compile(query);
            var sql = result.Sql;
            var parameters = new DynamicParameters(result.NamedBindings);

            return _connection.Query<ProductCodeEntity>(sql, param: parameters, transaction: _transaction).ToList();
        }
        public List<ProductCodeEntity> GetOverlappingProductCode(string hscode, string ProductCode, DateTime effectiveFromDt, DateTime effectiveThruDt)
        {
            var query = new Query("ProductCode")
                .Where("HSCode", "=", hscode)
                .Where("ProductCode", "=", ProductCode)
                .WhereRaw("((EffectiveFromDt BETWEEN '" + effectiveFromDt + "' AND '" + effectiveThruDt + "') OR (EffectiveThruDt BETWEEN '" + effectiveFromDt + "' AND '" + effectiveThruDt + "'))")
                .Select("*");


            var result = _sqlCompiler.Compile(query);
            var sql = result.Sql;
            var parameters = new DynamicParameters(result.NamedBindings);

            return _connection.Query<ProductCodeEntity>(sql, param: parameters, transaction: _transaction).ToList();
        }
        #endregion
    }
}
