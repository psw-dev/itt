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
        List<ViewRegulatedHsCode> GetRegulatedHsCodeList(int agencyId, int tradeTranTypeId);
        List<ViewRegulatedHsCode> GetRegulatedHsCodeList(int agencyId);
        List<ViewRegulatedHsCode> GetRegulatedHsCodeList();
        List<HscodeDetails> GetRegulatedHsCodeList(int tradeTranTypeId, int agencyId,  string hsCode);
        List<(string,int)>  ValidateRegulatedHSCodes(List<string> HSCodes, int agencyId, int tradeTranTypeId);
        List<ProductCodeAgencyLink> GetAgencyAssociatedHsCodeList(int agencyId, int tradeTranTypeId);
    }
}
