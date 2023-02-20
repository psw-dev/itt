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
                         WHERE ((EffectiveFromDt <= GetDate() AND EffectiveThruDt >= GetDate()) 
                         or (EffectiveFromDt >= GetDate() AND EffectiveThruDt >= GetDate()))
                         AND SoftDelete = 0";

            return _connection.Query<GetProductCodeListWithAgenciesResponseDTO>(
                    query,
                    transaction: _transaction
                   ).ToList();
        }
        public List<int> GetAllOTORoleIDAssociatedWithProductCode(long productCodeID)
        {
            var query = @"Select R.ID FROM [AUTH].[dbo].[UserRole] R
                        INNER JOIN [AUTH].[dbo].[AspNetUser] U ON R.AspNetUserID = U.ID
                        WHERE R.RoleCode = 'OTO'
                        AND AgencyID in (SELECT [AgencyID] FROM [ITT].[dbo].[ProductCodeAgencyLink] WHERE ProductCodeID = @PRODUCTCODEID AND (EffectiveFromDt <= GETDATE() AND EffectiveThruDt >= GETDATE()) )";

            return _connection.Query<int>(
              query, param: new { @PRODUCTCODEID = productCodeID },
              transaction: _transaction
             ).ToList();
        }

        public List<ViewRegulatedHsCodeExt> GetHsCodeExtList(int agencyId, string chapter)
        {
             var query = @"SELECT b.HsCodeExt, c.Description AS [HsCodeDescription], b.Description AS [HsCodeDescriptionExt], a.AgencyID 
                            FROM ProductCodeAgencyLink a JOIN ProductCode b on a.ProductCodeID = b.ID 
                            JOIN SHRD..Ref_HS_Codes c ON b.HSCode = c.HS_CODE
                            WHERE GETDATE() BETWEEN b.EFFECTIVEFROMDT AND b.EFFECTIVETHRUDT 
                            AND GETDATE() BETWEEN a.EFFECTIVEFROMDT AND a.EFFECTIVETHRUDT 
                            AND GETDATE() BETWEEN c.Effective_Date AND c.End_Date
                            AND a.IsActive=1 
                            AND a.AGENCYID = @AGENCYID AND b.ChapterCode = @CHAPTERCODE";
            return _connection.Query<ViewRegulatedHsCodeExt>(query, param: new { @AGENCYID = agencyId, @CHAPTERCODE = chapter },
              transaction: _transaction
             ).ToList();
        }

        public List<ViewRegulatedHsCodeExt> GetHsCodeExtList(int agencyId)
        {
             var query = @"SELECT b.HsCodeExt, c.Description AS [HsCodeDescription], b.Description AS [HsCodeDescriptionExt], a.AgencyID 
                            FROM ProductCodeAgencyLink a JOIN ProductCode b on a.ProductCodeID = b.ID 
                            JOIN SHRD..Ref_HS_Codes c ON b.HSCode = c.HS_CODE
                            WHERE GETDATE() BETWEEN b.EFFECTIVEFROMDT AND b.EFFECTIVETHRUDT 
                            AND GETDATE() BETWEEN a.EFFECTIVEFROMDT AND a.EFFECTIVETHRUDT 
                            AND GETDATE() BETWEEN c.Effective_Date AND c.End_Date
                            AND a.IsActive=1 
                            AND a.AGENCYID = @AGENCYID ";
            return _connection.Query<ViewRegulatedHsCodeExt>(query, param: new { @AGENCYID = agencyId},
              transaction: _transaction
             ).ToList();
        }

        public List<ViewRegulatedHsCodeExt> GetHsCodeExtList()
        {
             var query = @"SELECT b.HsCodeExt, c.Description AS [HsCodeDescription], b.Description AS [HsCodeDescriptionExt], a.AgencyID 
                            FROM ProductCodeAgencyLink a JOIN ProductCode b on a.ProductCodeID = b.ID 
                            JOIN SHRD..Ref_HS_Codes c ON b.HSCode = c.HS_CODE
                            WHERE GETDATE() BETWEEN b.EFFECTIVEFROMDT AND b.EFFECTIVETHRUDT 
                            AND GETDATE() BETWEEN a.EFFECTIVEFROMDT AND a.EFFECTIVETHRUDT 
                            AND GETDATE() BETWEEN c.Effective_Date AND c.End_Date
                            AND a.IsActive=1";
            return _connection.Query<ViewRegulatedHsCodeExt>(query,
              transaction: _transaction
             ).ToList();
        }

        
        public List<ViewRegulatedHsCode> GetRegulatedHsCodeList()
        {
             var query = @"SELECT HsCode, AgencyID 
                        FROM ProductCodeAgencyLink a JOIN ProductCode b on a.ProductCodeID = b.ID 
                        WHERE 
                        GETDATE() BETWEEN b.EFFECTIVEFROMDT AND b.EFFECTIVETHRUDT 
                        AND GETDATE() BETWEEN a.EFFECTIVEFROMDT AND a.EFFECTIVETHRUDT 
                        AND a.IsActive=1 ";
            return _connection.Query<ViewRegulatedHsCode>(query,
              transaction: _transaction
             ).ToList();
        }
        public List<ViewRegulatedHsCode> GetRegulatedHsCodeList(int agencyId)
        {
             var query = @"SELECT HsCode, AgencyID 
                        FROM ProductCodeAgencyLink a JOIN ProductCode b on a.ProductCodeID = b.ID 
                        WHERE a.AGENCYID = @AGENCYID 
                        AND GETDATE() BETWEEN b.EFFECTIVEFROMDT AND b.EFFECTIVETHRUDT 
                        AND GETDATE() BETWEEN a.EFFECTIVEFROMDT AND a.EFFECTIVETHRUDT 
                        AND a.IsActive=1 ";
            return _connection.Query<ViewRegulatedHsCode>(query, param: new { @AGENCYID = agencyId},
              transaction: _transaction
             ).ToList();
        }
        public List<ViewRegulatedHsCode> GetRegulatedHsCodeList(int agencyId, int tradeTranTypeId)
        {
             var query = @"SELECT HsCode, AgencyID 
                        FROM ProductCodeAgencyLink a JOIN ProductCode b on a.ProductCodeID = b.ID 
                        WHERE a.AGENCYID = @AGENCYID  AND b.TradeTranTypeID = @TRADETRANTYPEID
                        AND GETDATE() BETWEEN b.EFFECTIVEFROMDT AND b.EFFECTIVETHRUDT 
                        AND GETDATE() BETWEEN a.EFFECTIVEFROMDT AND a.EFFECTIVETHRUDT 
                        AND a.IsActive=1 ";
            return _connection.Query<ViewRegulatedHsCode>(query, param: new { @AGENCYID = agencyId, @TRADETRANTYPEID =tradeTranTypeId},
              transaction: _transaction
             ).ToList();
        }
        public List<HscodeDetails> GetRegulatedHsCodeList(int tradeTranTypeId, int agencyId, string hsCode)
        {
             var query = @"SELECT a.ID AS ProductCodeAgencyLinkID, c.Description AS ITEMDESCRIPTION, b.ProductCode ,b.Description AS ITEMDESCRIPTIONEXT
                            FROM ProductCodeAgencyLink a 
                            JOIN ProductCode b on a.ProductCodeID = b.ID 
                            JOIN SHRD..Ref_HS_Codes c ON b.HSCode = c.HS_CODE
                            WHERE b.HSCode=@HSCODE AND a.AGENCYID = @AGENCYID AND b.TradeTranTypeID = @TRADETRANTYPEID
                            AND GETDATE() BETWEEN b.EFFECTIVEFROMDT AND b.EFFECTIVETHRUDT 
                            AND GETDATE() BETWEEN a.EFFECTIVEFROMDT AND a.EFFECTIVETHRUDT 
                            AND GETDATE() BETWEEN c.Effective_Date AND c.End_Date
                            AND a.IsActive=1";
            return _connection.Query<HscodeDetails>(query, param: new { @AGENCYID = agencyId, @TRADETRANTYPEID =tradeTranTypeId, @HSCODE= hsCode},
              transaction: _transaction
             ).ToList();
        }
        #endregion
    }
}
