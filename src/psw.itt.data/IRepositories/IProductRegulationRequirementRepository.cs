using System.Collections.Generic;
using PSW.ITT.Data.DTO;
using PSW.ITT.Data.Entities;

namespace PSW.ITT.Data.IRepositories
{
    public interface IProductRegulationRequirementRepository : IRepository<ProductRegulationRequirement>
    {
        List<GetRegulatoryDataDTO> GetRegulatoryDataByTradeTypeAndAgency(short TradeTranTypeID, short AgencyID);

    }
}
