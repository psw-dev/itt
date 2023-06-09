using System;
using System.Collections.Generic;
using PSW.ITT.Data.DTO;
using PSW.ITT.Data.Entities;

namespace PSW.ITT.Data.IRepositories
{
    public interface IProductCodeEntityRepository : IRepository<ProductCodeEntity>
    {
        List<ProductCodeEntity> GetActiveProductCode();
        List<ProductCodeEntity> GetActiveAgencyProductCode(int agencyID, short tradeTranTypeID);
        List<LOVItem> GetActiveAgencyProductCodeLOV(int agencyID, short tradeTranTypeID, string lovTableName, string lovColumnName);
        List<LOVItem> GetDocumentLOV(int agencyID, string lovTableName, string lovColumnName, int TradeTranTypeID);

        List<ProductCodeEntity> GetOverlappingProductCode(string hscode, string ProductCode, DateTime effectiveFromDt, DateTime effectiveThruDt, short tradeType);
        List<ProductCodeAgencyLink> GetProductCodeValidity(string ProductCode, int AgencyID, short tradeType);
        List<GetProductExcelDataDTO> GetProductExcelData();
        List<ProductDetail> GetPCTCodeList(int tradeTranTypeID, string hsCode);
        ProductCodeWithAgencyLink GetActiveProductCodeDetail(int agencyID, short tradeTranTypeID, string hsCodeExt);
    }
}
