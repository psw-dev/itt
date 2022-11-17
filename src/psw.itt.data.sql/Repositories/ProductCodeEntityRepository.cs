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
        public List<ProductCodeEntity> GetActiveAgencyProductCode(int agencyID, short tradeTranTypeID)
        {
            var query = new Query("ProductCode")
              .Join("ProductCodeAgencyLink", "ProductCodeAgencyLink.ProductCodeID", "ProductCode.ID")
              .WhereRaw("ProductCodeAgencyLink.AgencyID = " + agencyID)
              .WhereRaw("(ProductCode.TradeTranTypeID = " + tradeTranTypeID + " OR ProductCode.TradeTranTypeID = 4)")
              .WhereRaw("((ProductCodeAgencyLink.EffectiveFromDt <= GetDate() AND ProductCodeAgencyLink.EffectiveThruDt >= GetDate())")
              .OrWhereRaw("(ProductCodeAgencyLink.EffectiveFromDt >= GetDate() AND ProductCodeAgencyLink.EffectiveThruDt >= GetDate()))")
              .SelectRaw(" ROW_NUMBER() OVER(Order By(Select 1)) as SerialID, ProductCodeAgencyLink.ID,ChapterCode,HSCode,HSCodeExt,ProductCode.ProductCode,ProductCode.[Description],ProductCode.TradeTranTypeID,ProductCodeAgencyLink.EffectiveFromDt, ProductCodeAgencyLink.EffectiveThruDt, ProductCodeAgencyLink.IsActive")
              .OrderBy("ProductCodeAgencyLink.EffectiveThruDt");

            var result = _sqlCompiler.Compile(query);
            var sql = result.Sql;
            var parameters = new DynamicParameters(result.NamedBindings);

            return _connection.Query<ProductCodeEntity>(sql, param: parameters, transaction: _transaction).ToList();
        }

        public List<LOVItem> GetActiveAgencyProductCodeLOV(int agencyID, short tradeTranTypeID, string lovTableName, string lovColumnName)
        {
            return _connection.Query<LOVItem>(string.Format("SELECT ProductCode.ID  as ItemKey, {0} as ItemValue FROM [ITT].[dbo].[{1}] INNER JOIN [ProductCodeAgencyLink] ON [ProductCodeAgencyLink].[ProductCodeID] = [{1}].[ID]  WHERE ProductCodeAgencyLink.AgencyID = {2} and (TradeTranTypeID = 4 or TradeTranTypeID = {3}) AND ((ProductCodeAgencyLink.EffectiveFromDt <= GetDate() AND ProductCodeAgencyLink.EffectiveThruDt >= GetDate()) OR (ProductCodeAgencyLink.EffectiveFromDt >= GetDate() AND ProductCodeAgencyLink.EffectiveThruDt >= GetDate())) ", lovColumnName, lovTableName, agencyID, tradeTranTypeID)).ToList();
        }
        public List<LOVItem> GetDocumentLOV(int agencyID, string lovTableName, string lovColumnName)
        {
            return _connection.Query<LOVItem>(string.Format("SELECT Code as ItemKey, {0} as ItemValue FROM [SHRD].[dbo].[{1}] WHERE AgencyID = {2} ", lovColumnName, lovTableName, agencyID)).ToList();
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
        #endregion
    }
}
