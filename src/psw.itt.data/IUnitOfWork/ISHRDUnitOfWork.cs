using PSW.ITT.Data.IRepositories;
using PSW.RabbitMq;
using System;

namespace PSW.ITT.Data
{
    public interface ISHRDUnitOfWork : IDisposable
    {

        #region Repositories
        IAgencyRepository AgencyRepository { get; }
        IAppConfigRepository AppConfigRepository { get; }
        IAttachedObjectFormatRepository AttachedObjectFormatRepository { get; }
        IAttachmentStatusRepository AttachmentStatusRepository { get; }
        ICityRepository CityRepository { get; }
        ICountryRepository CountryRepository { get; }
        ICountrySubEntityRepository CountrySubEntityRepository { get; }
        ICurrencyRepository CurrencyRepository { get; }
        IDialingCodeRepository DialingCodeRepository { get; }
        IDocumentTypeRepository DocumentTypeRepository { get; }
        IGenderRepository GenderRepository { get; }
        IMinistryRepository MinistryRepository { get; }
        IPortRepository PortRepository { get; }
        ITradePurposeRepository TradePurposeRepository { get; }
        ITradeTranTypeRepository TradeTranTypeRepository { get; }
        IUoMRepository UoMRepository { get; }
        IZoneRepository ZoneRepository { get; }

		// IAttributeValidationMappingRepository AttributeValidationMappingRepository { get; }
        // IProductCodeAgencyLinkRepository ProductCodeAgencyLinkRepository { get; }
        // IProductCodeChapterRepository ProductCodeChapterRepository { get; }
        // IProductCodeEntityRepository ProductCodeEntityRepository { get; }
        // IProductCodeSheetUploadHistoryRepository ProductCodeSheetUploadHistoryRepository { get; }
        // IProductCodeSheetUploadStatusRepository ProductCodeSheetUploadStatusRepository { get; }
        // ISheetAttributeMappingRepository SheetAttributeMappingRepository { get; }
        // ILPCORegulationRepository LPCORegulationRepository { get; }
        // IProductRegulationRequirementRepository ProductRegulationRequirementRepository { get; }
        IShrdCommonForLovRepository ShrdCommonForLovRepository { get; }

        IEventBus eventBus { get; }
        #endregion

        #region Methods
       
        void OpenConnection();
        void CloseConnection();

        #endregion
    }
}
