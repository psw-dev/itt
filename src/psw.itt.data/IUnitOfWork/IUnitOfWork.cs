using PSW.ITT.Data.IRepositories;
using PSW.RabbitMq;
using System;

namespace PSW.ITT.Data
{
    public interface IUnitOfWork : IDisposable
    {

        #region Repositories
		IAttributeValidationMappingRepository AttributeValidationMappingRepository { get; }
		ICalculationBasisRepository CalculationBasisRepository { get; }
		ICalculationSourceRepository CalculationSourceRepository { get; }
        IProductCodeAgencyLinkRepository ProductCodeAgencyLinkRepository { get; }
        IProductCodeChapterRepository ProductCodeChapterRepository { get; }
        IProductCodeEntityRepository ProductCodeEntityRepository { get; }
        IProductCodeSheetUploadHistoryRepository ProductCodeSheetUploadHistoryRepository { get; }
        IProductCodeSheetUploadStatusRepository ProductCodeSheetUploadStatusRepository { get; }
        ISheetAttributeMappingRepository SheetAttributeMappingRepository { get; }
        ILPCORegulationRepository LPCORegulationRepository { get; }
        IProductRegulationRequirementRepository ProductRegulationRequirementRepository { get; }
        ILPCOFeeStructureRepository LPCOFeeStructureRepository { get; }
        ISheetTypeRepository SheetTypeRepository { get; }
        ICommonForLovRepository CommonForLovRepository { get; }
		IValidationRepository ValidationRepository { get; }
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
