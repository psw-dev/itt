using PSW.ITT.Data.Entities;

using System.Collections.Generic;
namespace PSW.ITT.Data.IRepositories
{
    public interface IProductCodeSheetUploadHistoryRepository : IRepository<ProductCodeSheetUploadHistory>
    {
        void SetIsCurrent(List<int> SheetTypeIDs);
        List<ProductCodeSheetUploadHistory> GetFilesBySheetType(int AgencyId, List<int> SheetTypeIDs);
        List<ProductCodeSheetUploadHistory> GetFilesHistoryBySheetType(List<int> SheetTypeIDs);
    }
}
