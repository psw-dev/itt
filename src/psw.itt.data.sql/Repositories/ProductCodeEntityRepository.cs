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
              .SelectRaw(" ROW_NUMBER() OVER(Order By(Select 1)) as SerialID, *")
              .OrderBy("EffectiveThruDt");

            var result = _sqlCompiler.Compile(query);
            var sql = result.Sql;
            var parameters = new DynamicParameters(result.NamedBindings);

            return _connection.Query<ProductCodeEntity>(sql, param: parameters, transaction: _transaction).ToList();
        }
        public List<ProductCodeEntity> GetActiveAgencyProductCode(int agencyID)
        {
            var query = new Query("ProductCode")
              .Join("ProductCodeAgencyLink", "ProductCodeAgencyLink.ProductCodeID", "ProductCode.ID")
              .WhereRaw("ProductCodeAgencyLink.AgencyID = " + agencyID)
              .WhereRaw("((ProductCodeAgencyLink.EffectiveFromDt <= GetDate() AND ProductCodeAgencyLink.EffectiveThruDt >= GetDate())")
              .OrWhereRaw("(ProductCodeAgencyLink.EffectiveFromDt >= GetDate() AND ProductCodeAgencyLink.EffectiveThruDt >= GetDate()))")
              .SelectRaw(" ROW_NUMBER() OVER(Order By(Select 1)) as SerialID, ProductCodeAgencyLink.ID,ChapterCode,HSCode,HSCodeExt,ProductCode.ProductCode,ProductCode.[Description],ProductCode.TradeTranTypeID,ProductCodeAgencyLink.EffectiveFromDt, ProductCodeAgencyLink.EffectiveThruDt, ProductCodeAgencyLink.IsActive")
              .OrderBy("ProductCodeAgencyLink.EffectiveThruDt");

            var result = _sqlCompiler.Compile(query);
            var sql = result.Sql;
            var parameters = new DynamicParameters(result.NamedBindings);

            return _connection.Query<ProductCodeEntity>(sql, param: parameters, transaction: _transaction).ToList();
        }
        public List<ProductCodeEntity> GetOverlappingProductCode(string hscode, string ProductCode, DateTime effectiveFromDt, DateTime effectiveThruDt, short tradeType)
        {
            var query = new Query("ProductCode")
               .Where("HSCode", "=", hscode)
               .Where("ProductCode", "=", ProductCode)
               .WhereRaw("(TradeTranTypeID = 4 or TradeTranTypeID = " + tradeType + ")")
               .WhereRaw("((EffectiveFromDt BETWEEN '" + effectiveFromDt + "' AND '" + effectiveThruDt + "') OR (EffectiveThruDt BETWEEN '" + effectiveFromDt + "' AND '" + effectiveThruDt + "'))")
               .Select("*");
            if (tradeType == 4)
            {
                query = new Query("ProductCode")
                               .Where("HSCode", "=", hscode)
                               .Where("ProductCode", "=", ProductCode)
                               .WhereRaw("(TradeTranTypeID != 4 or TradeTranTypeID = " + tradeType + ")")
                               .WhereRaw("((EffectiveFromDt BETWEEN '" + effectiveFromDt + "' AND '" + effectiveThruDt + "') OR (EffectiveThruDt BETWEEN '" + effectiveFromDt + "' AND '" + effectiveThruDt + "'))")
                               .Select("*");
            }



            var result = _sqlCompiler.Compile(query);
            var sql = result.Sql;
            var parameters = new DynamicParameters(result.NamedBindings);

            return _connection.Query<ProductCodeEntity>(sql, param: parameters, transaction: _transaction).ToList();
        }
        public bool GetProductCodeValidity(string ProductCode, int AgencyID)
        {
            var query = new Query("ProductCode")
              .Join("ProductCodeAgencyLink", "ProductCodeAgencyLink.ProductCodeID", "ProductCode.ID")
              .WhereRaw("ProductCode.HSCodeExt = " + ProductCode)
              .WhereRaw("ProductCodeAgencyLink.AgencyID = " + AgencyID)
              .WhereRaw("((ProductCodeAgencyLink.EffectiveFromDt <= GetDate() AND ProductCodeAgencyLink.EffectiveThruDt >= GetDate())")
              .OrWhereRaw("(ProductCodeAgencyLink.EffectiveFromDt >= GetDate() AND ProductCodeAgencyLink.EffectiveThruDt >= GetDate()))")
              .SelectRaw("ProductCode.HSCodeExt");

            var result = _sqlCompiler.Compile(query);
            var sql = result.Sql;
            var parameters = new DynamicParameters(result.NamedBindings);

            var returnValue = _connection.Query<string>(sql, param: parameters, transaction: _transaction).ToList();
            return  returnValue.Count < 1 ? false : true;
        }
        #endregion
    }
}
