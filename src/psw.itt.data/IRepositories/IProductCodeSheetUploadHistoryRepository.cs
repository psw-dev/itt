using PSW.ITT.Data.Entities;

namespace PSW.ITT.Data.IRepositories
{
    public interface IProductCodeSheetUploadHistoryRepository : IRepository<ProductCodeSheetUploadHistory>
    {
        void SetIsCurrent(short AgencyID);
    }
}
