using PSW.ITT.Data.IRepositories;
using PSW.RabbitMq;
using System;

namespace PSW.ITT.Data
{
    public interface IUnitOfWork : IDisposable
    {

        #region Repositories
		IAttributeValidationMappingRepository AttributeValidationMappingRepository { get; }
        IProductCodeAgencyLinkRepository ProductCodeAgencyLinkRepository { get; }
        IProductCodeChapterRepository ProductCodeChapterRepository { get; }
        IProductCodeEntityRepository ProductCodeEntityRepository { get; }
        IProductCodeSheetUploadHistoryRepository ProductCodeSheetUploadHistoryRepository { get; }
        IProductCodeSheetUploadStatusRepository ProductCodeSheetUploadStatusRepository { get; }
        ISheetAttributeMappingRepository SheetAttributeMappingRepository { get; }
        ILPCORegulationRepository LPCORegulationRepository { get; }
        IProductRegulationRequirementRepository ProductRegulationRequirementRepository { get; }
        ILPCOFeeStructureRepository LPCOFeeStructureRepository { get; }
        IEventBus eventBus { get; }
        #endregion

        #region Methods
        void Commit();
        void BeginTransaction();
        void Rollback();

        void OpenConnection();
        void CloseConnection();

        #endregion
    }
}
