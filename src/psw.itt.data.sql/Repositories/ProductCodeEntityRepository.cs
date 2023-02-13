using System;
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
              .WhereRaw("SoftDelete = 0 AND ProductCodeAgencyLink.AgencyID = " + agencyID)
              .WhereRaw("(ProductCode.TradeTranTypeID = " + tradeTranTypeID + " OR ProductCode.TradeTranTypeID = 4)")
              .WhereRaw("((ProductCodeAgencyLink.EffectiveFromDt <= GetDate() AND ProductCodeAgencyLink.EffectiveThruDt >= GetDate())")
              .OrWhereRaw("(ProductCodeAgencyLink.EffectiveFromDt >= GetDate() AND ProductCodeAgencyLink.EffectiveThruDt >= GetDate()))")
              .SelectRaw(" ROW_NUMBER() OVER(Order By(Select 1)) as SerialID, ProductCodeAgencyLink.ID,ChapterCode,HSCode,HSCodeExt,ProductCode.ProductCode,ProductCode.[Description],ProductCode.TradeTranTypeID,ProductCodeAgencyLink.EffectiveFromDt, ProductCodeAgencyLink.EffectiveThruDt, ProductCodeAgencyLink.IsActive")
              .SelectRaw("CASE WHEN EXISTS( SELECT * FROM[LPCORegulation] L WHERE L.ProductCodeAgencyLinkID =  [ProductCodeAgencyLink].ID  And (L.EffectiveFromDt <= GetDate() AND L.EffectiveThruDt >= GetDate())) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END as Regulated")
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
        public List<LOVItem> GetDocumentLOV(int agencyID, string lovTableName, string lovColumnName, int TradeTranTypeID)
        {
            return _connection.Query<LOVItem>(string.Format("SELECT  Code as ItemKey, {0} as ItemValue FROM [ITT].[dbo].[ITTAgencyDocuments] AD INNER JOIN [SHRD].[dbo].[DocumentType] DT ON AD.DocumentTypeCode = DT.Code WHERE IsActive = 1 AND (AD.AgencyID = {2} or AD.AgencyID is null) AND (TradeTranTypeID = {3} or TradeTranTypeID is null)", lovColumnName, lovTableName, agencyID, TradeTranTypeID)).ToList();
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
        public List<ProductCodeAgencyLink> GetProductCodeValidity(string ProductCode, int AgencyID, short tradeType)
        {
            var query = new Query("ProductCode")
              .Join("ProductCodeAgencyLink", "ProductCodeAgencyLink.ProductCodeID", "ProductCode.ID")
              .WhereRaw("ProductCode.HSCodeExt = '" + ProductCode + "'")
              .WhereRaw("ProductCodeAgencyLink.AgencyID = " + AgencyID)
              .WhereRaw("(TradeTranTypeID != 4 or TradeTranTypeID = " + tradeType + ")")
              .WhereRaw("((ProductCodeAgencyLink.EffectiveFromDt <= GetDate() AND ProductCodeAgencyLink.EffectiveThruDt >= GetDate())")
              .OrWhereRaw("(ProductCodeAgencyLink.EffectiveFromDt >= GetDate() AND ProductCodeAgencyLink.EffectiveThruDt >= GetDate()))")
              .SelectRaw("ProductCodeAgencyLink.*");

            var result = _sqlCompiler.Compile(query);
            var sql = result.Sql;
            var parameters = new DynamicParameters(result.NamedBindings);

            var returnValue = _connection.Query<ProductCodeAgencyLink>(sql, param: parameters, transaction: _transaction).ToList();
            return returnValue;
        }

        public List<GetProductExcelDataDTO> GetProductExcelData()
        {

            var query = @" SELECT ID, [ChapterCode]
      ,[HSCode]
      ,[ProductCode]
      ,[Description]
      ,  (SELECT STRING_AGG(A.Code, ', ') FROM [ITT].[dbo].[ProductCodeAgencyLink] P INNER join SHRD..Agency A on A.ID = P.AgencyID WHERE P.ProductCodeID = PC.ID and ((EffectiveFromDt <= GetDate() AND EffectiveThruDt >= GetDate()) OR (EffectiveFromDt >= GetDate() AND EffectiveThruDt >= GetDate())) and SoftDelete = 0 ) as Agencies

      ,[TradeTranTypeID]
      ,'Active' as Status
  FROM [ITT].[dbo].[ProductCode] PC
 WHERE ((EffectiveFromDt <= GetDate() AND EffectiveThruDt >= GetDate()) OR (EffectiveFromDt >= GetDate() AND EffectiveThruDt >= GetDate()))";

            return _connection.Query<GetProductExcelDataDTO>(
                    query,
                    transaction: _transaction
                   ).ToList();
        }
        public List<ProductDetail> GetPCTCodeList(int tradeTranTypeID, string hsCode)
        {

            var query = @"SELECT ID, PRODUCTCODE, Concat(ProductCode, Description) as  ItemDescription  
                        FROM ProductCode WHERE GETDATE() BETWEEN EFFECTIVEFROMDT AND EFFECTIVETHRUDT 
                        AND  tradeTranTypeId = @TRADETRANTYPEID
                        AND  HsCode = @HSCODE ";

            return _connection.Query<ProductDetail>(
                    query, param: new { TRADETRANTYPEID = tradeTranTypeID, HSCODE = hsCode},
                    transaction: _transaction
                   ).ToList();
        }
        public List<ProductCodeWithAgencyLink> GetActiveProductCodeDetail(int agencyID, short tradeTranTypeID, string hsCodeExt)
        {
            var query = new Query("ProductCode")
              .Join("ProductCodeAgencyLink", "ProductCodeAgencyLink.ProductCodeID", "ProductCode.ID")
              .WhereRaw("SoftDelete = 0 AND ProductCodeAgencyLink.AgencyID = " + agencyID)
              .WhereRaw("ProductCode.HSCodeExt= " + hsCodeExt)
              .WhereRaw("(ProductCode.TradeTranTypeID = " + tradeTranTypeID + " OR ProductCode.TradeTranTypeID = 4) " )
              .WhereRaw("((ProductCodeAgencyLink.EffectiveFromDt <= GetDate() AND ProductCodeAgencyLink.EffectiveThruDt >= GetDate())")
              .OrWhereRaw("(ProductCodeAgencyLink.EffectiveFromDt >= GetDate() AND ProductCodAgencyLink.EffectiveThruDt >= GetDate()))")
              .WhereRaw("ProductCodeAgencyLink.IsActive= 1 AND ProductCodeAgencyLink.SoftDelete = 0")
              .Select("ProductCode.*")
              .Select("ProductCodeAgencyLink.ID AS ProductCodeAgencyLinkID, ProductCodeAgencyLink.AgencyID, ProductCodeAgencyLink.EffectiveFromDt AS ProductCodeAgencyLinkEffectiveFromDt, ProductCodeAgencyLink.EffectiveThruDt AS ProductCodeAgencyLinkEffectiveThruDt, ProductCodeAgencyLink.RegulationEffectiveFromDt, ProductCodeAgencyLink.RegulationEffectiveThruDt, ProductCodeAgencyLink.IsActive, ProductCodeAgencyLink.SoftDelete");

            var result = _sqlCompiler.Compile(query);
            var sql = result.Sql;
            var parameters = new DynamicParameters(result.NamedBindings);

            return _connection.Query<ProductCodeEntity>(sql, param: parameters, transaction: _transaction).ToList();
        }
        #endregion
    }
}
