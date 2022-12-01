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