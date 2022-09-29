using System;
using System.Collections.Generic;
using PSW.ITT.Data.Entities;

namespace PSW.ITT.Data.IRepositories
{
    public interface IProductCodeEntityRepository : IRepository<ProductCodeEntity>
    {
        List<ProductCodeEntity> GetActiveProductCode();
        List<ProductCodeEntity> GetActiveAgencyProductCode(int agencyID);
        List<ProductCodeEntity> GetOverlappingProductCode(string hscode, string ProductCode, DateTime effectiveFromDt, DateTime effectiveThruDt, short tradeType);
    }
}
