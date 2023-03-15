using Dapper;
using Microsoft.Extensions.Configuration;
using PSW.ITT.Common;
using PSW.ITT.Data.IRepositories;
using PSW.ITT.Data.Sql.Repositories;
using PSW.RabbitMq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace PSW.ITT.Data.Sql.UnitOfWork
{
    public class SHRDUnitOfWork : ISHRDUnitOfWork
    {
        #region Private properties 
        private IDbConnection _connection;
        private IDbTransaction _transaction;


        // private bool _disposed;

        // Repositories 
        private IAgencyRepository _agencyRepository;
        private IAppConfigRepository _appConfigRepository;
        private IAttachedObjectFormatRepository _attachedObjectFormatRepository;
        private IAttachmentStatusRepository _attachmentStatusRepository;
        private ICityRepository _cityRepository;
        private ICountryRepository _countryRepository;
        private ICountrySubEntityRepository _countrySubEntityRepository;
        private ICurrencyRepository _currencyRepository;
        private IDialingCodeRepository _dialingCodeRepository;
        private IDocumentTypeRepository _documentTypeRepository;
        private IGenderRepository _genderRepository;
        private IMinistryRepository _ministryRepository;
        private IPortRepository _portRepository;
        private ITradePurposeRepository _tradePurposeRepository;
        private ITradeTranTypeRepository _tradeTranTypeRepository;
        private IUoMRepository _uoMRepository;
        private IZoneRepository _zoneRepository;
        private IRef_UnitsRepository _ref_UnitsRepository;
		// private IAttributeValidationMappingRepository _attributeValidationMappingRepository;
        // private IProductCodeAgencyLinkRepository _productCodeAgencyLinkRepository;
        // private IProductCodeChapterRepository _productCodeChapterRepository;
        // private IProductCodeEntityRepository _productCodeEntityRepository;
        // private IProductCodeSheetUploadHistoryRepository _productCodeSheetUploadHistoryRepository;
        // private IProductCodeSheetUploadStatusRepository _productCodeSheetUploadStatusRepository;
        // private ISheetAttributeMappingRepository _sheetAttributeMappingRepository;
        // private ILPCORegulationRepository _lPCORegulationRepository;
        // private IProductRegulationRequirementRepository _productRegulationRequirementRepository;
		// private IValidationRepository _validationRepository;
		private IShrdCommonForLovRepository _shrdCommonForLovRepository;

        private IEventBus _eventBus;
        public IEventBus eventBus => _eventBus;
        private IConfiguration _configuration;
        #endregion

        #region Constructors & Destructors

        public SHRDUnitOfWork(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new SqlConnection(_configuration.GetConnectionString("SHRDConnectionString"));
        }
        public SHRDUnitOfWork(IConfiguration configuration, IEventBus eventBus)
        {
            _eventBus = eventBus;
            _configuration = configuration;
            _connection = new SqlConnection(_configuration.GetConnectionString("SHRDConnectionString"));
        }

        public SHRDUnitOfWork(string connectionString)
        {
            // TODO : Get Connection String From appSetting.json
            // string connectionString = "Server=127.0.0.1;Initial Catalog=ITT;User ID=sa;Password=@Password1;";
            _connection = new SqlConnection(connectionString);
        }
        // ~UnitOfWork()
        // {
        //     dispose(false);
        // }

        #endregion


        #region Public Properties
        public IAgencyRepository AgencyRepository => _agencyRepository ?? (_agencyRepository = new AgencyRepository(_connection));
        public IAppConfigRepository AppConfigRepository => _appConfigRepository ?? (_appConfigRepository = new AppConfigRepository(_connection));
        public IAttachedObjectFormatRepository AttachedObjectFormatRepository => _attachedObjectFormatRepository ?? (_attachedObjectFormatRepository = new AttachedObjectFormatRepository(_connection));
        public IAttachmentStatusRepository AttachmentStatusRepository => _attachmentStatusRepository ?? (_attachmentStatusRepository = new AttachmentStatusRepository(_connection));
        public ICityRepository CityRepository => _cityRepository ?? (_cityRepository = new CityRepository(_connection));
        public ICountryRepository CountryRepository => _countryRepository ?? (_countryRepository = new CountryRepository(_connection));
        public ICountrySubEntityRepository CountrySubEntityRepository => _countrySubEntityRepository ?? (_countrySubEntityRepository = new CountrySubEntityRepository(_connection));
        public ICurrencyRepository CurrencyRepository => _currencyRepository ?? (_currencyRepository = new CurrencyRepository(_connection));
        public IDialingCodeRepository DialingCodeRepository => _dialingCodeRepository ?? (_dialingCodeRepository = new DialingCodeRepository(_connection));
        public IDocumentTypeRepository DocumentTypeRepository => _documentTypeRepository ?? (_documentTypeRepository = new DocumentTypeRepository(_connection));
        public IGenderRepository GenderRepository => _genderRepository ?? (_genderRepository = new GenderRepository(_connection));
        public IMinistryRepository MinistryRepository => _ministryRepository ?? (_ministryRepository = new MinistryRepository(_connection));
        public IPortRepository PortRepository => _portRepository ?? (_portRepository = new PortRepository(_connection));
        public ITradePurposeRepository TradePurposeRepository => _tradePurposeRepository ?? (_tradePurposeRepository = new TradePurposeRepository(_connection));
        public ITradeTranTypeRepository TradeTranTypeRepository => _tradeTranTypeRepository ?? (_tradeTranTypeRepository = new TradeTranTypeRepository(_connection));
        public IUoMRepository UoMRepository => _uoMRepository ?? (_uoMRepository = new UoMRepository(_connection));
        public IZoneRepository ZoneRepository => _zoneRepository ?? (_zoneRepository = new ZoneRepository(_connection));
        public IRef_UnitsRepository Ref_UnitsRepository => _ref_UnitsRepository ?? (_ref_UnitsRepository = new Ref_UnitsRepository(_connection));
        // public IAttributeValidationMappingRepository AttributeValidationMappingRepository => _attributeValidationMappingRepository ?? (_attributeValidationMappingRepository = new AttributeValidationMappingRepository(_connection));
		// public IProductCodeAgencyLinkRepository ProductCodeAgencyLinkRepository => _productCodeAgencyLinkRepository ?? (_productCodeAgencyLinkRepository = new ProductCodeAgencyLinkRepository(_connection));
        // public IProductCodeChapterRepository ProductCodeChapterRepository => _productCodeChapterRepository ?? (_productCodeChapterRepository = new ProductCodeChapterRepository(_connection));
        // public IProductCodeEntityRepository ProductCodeEntityRepository => _productCodeEntityRepository ?? (_productCodeEntityRepository = new ProductCodeEntityRepository(_connection));
        // public IProductCodeSheetUploadHistoryRepository ProductCodeSheetUploadHistoryRepository => _productCodeSheetUploadHistoryRepository ?? (_productCodeSheetUploadHistoryRepository = new ProductCodeSheetUploadHistoryRepository(_connection));
        // public IProductCodeSheetUploadStatusRepository ProductCodeSheetUploadStatusRepository => _productCodeSheetUploadStatusRepository ?? (_productCodeSheetUploadStatusRepository = new ProductCodeSheetUploadStatusRepository(_connection));
        // public ISheetAttributeMappingRepository SheetAttributeMappingRepository => _sheetAttributeMappingRepository ?? (_sheetAttributeMappingRepository = new SheetAttributeMappingRepository(_connection));
        // public ILPCORegulationRepository LPCORegulationRepository => _lPCORegulationRepository ?? (_lPCORegulationRepository = new LPCORegulationRepository(_connection));
        // public IProductRegulationRequirementRepository ProductRegulationRequirementRepository => _productRegulationRequirementRepository ?? (_productRegulationRequirementRepository = new ProductRegulationRequirementRepository(_connection));
        // public IValidationRepository ValidationRepository => _validationRepository ?? (_validationRepository = new ValidationRepository(_connection));
        public IShrdCommonForLovRepository ShrdCommonForLovRepository => _shrdCommonForLovRepository ?? (_shrdCommonForLovRepository = new ShrdCommonForLovRepository(_connection));

        #endregion

        public IDbConnection Connection
        {
            get
            {
                if (_connection == null) _connection = new SqlConnection(_configuration.GetConnectionString("SHRDConnectionString"));

                return _connection;
            }

            set
            {
                _connection = value;
            }
        }


        #region Views



        #endregion

        #region Stored Procedures



        #endregion

        #region Public constructors

        // public UnitOfWork()
        // {
        //     try
        //     {
        //         _connection.Open();
        //     }
        //     catch (Exception exception)
        //     {

        //         throw exception;
        //     } 
        // }

        #endregion

        #region Public methods

        public void Dispose()
        {
            if (_connection != null)
                _connection.Dispose();
        }

        public List<object> ExecuteQuery(string query)
        {
            return (List<object>)_connection.Query<List<object>>(query);
        }

        public void OpenConnection()
        {
            try
            {
                _connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void CloseConnection()
        {
            try
            {
                if (_connection != null && _connection.State != ConnectionState.Closed)
                    Connection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

     
        #endregion

    }
}