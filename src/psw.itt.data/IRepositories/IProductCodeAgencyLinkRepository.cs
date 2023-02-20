using System.Collections.Generic;
using PSW.ITT.Data.DTO;
using PSW.ITT.Data.Entities;

namespace PSW.ITT.Data.IRepositories
{
    public interface IProductCodeAgencyLinkRepository : IRepository<ProductCodeAgencyLink>
    {
        List<GetProductCodeListWithAgenciesResponseDTO> GetProductCodeIDWithOGA();
        List<int> GetAllOTORoleIDAssociatedWithProductCode(long productCodeID);
        List<ViewRegulatedHsCodeExt> GetHsCodeExtList(int agencyId, string chapter);
        List<ViewRegulatedHsCodeExt> GetHsCodeExtList(int agencyId);
        List<ViewRegulatedHsCodeExt> GetHsCodeExtList();
    }
}
