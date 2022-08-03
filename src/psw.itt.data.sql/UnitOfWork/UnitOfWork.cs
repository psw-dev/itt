using Dapper;
using Microsoft.Extensions.Configuration;
using psw.itt.common;
using psw.itt.data.IRepositories;
using psw.itt.data.sql.Repositories;
using PSW.RabbitMq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace psw.itt.data.sql.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        #region Private properties 
        private IDbConnection _connection;
        private IDbTransaction _transaction;


        // private bool _disposed;

        // Repositories 

        private IChapterAgencyLinkRepository _chapterAgencyLinkRepository;
        private IProductCodeChapterRepository _productCodeChapterRepository;
        private IProductCodeEntityRepository _productCodeEntityRepository;
        private IProductCodeSheetUploadHistoryRepository _productCodeSheetUploadHistoryRepository;
        private IProductCodeSheetUploadStatusRepository _productCodeSheetUploadStatusRepository;

        private IEventBus _eventBus;
        public IEventBus eventBus => _eventBus;
        private IConfiguration _configuration;
        #endregion

        #region Constructors & Destructors

        public UnitOfWork(IConfiguration configuration)
        {
            _configuration = configuration;
            _connection = new SqlConnection(_configuration.GetConnectionString("ITTConnectionString"));
        }
        public UnitOfWork(IConfiguration configuration, IEventBus eventBus)
        {
            _eventBus = eventBus;
            _configuration = configuration;
            _connection = new SqlConnection(_configuration.GetConnectionString("ITTConnectionString"));
        }

        public UnitOfWork(string connectionString)
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
        public IChapterAgencyLinkRepository ChapterAgencyLinkRepository => _chapterAgencyLinkRepository ?? (_chapterAgencyLinkRepository = new ChapterAgencyLinkRepository(_connection));
        public IProductCodeChapterRepository ProductCodeChapterRepository => _productCodeChapterRepository ?? (_productCodeChapterRepository = new ProductCodeChapterRepository(_connection));
        public IProductCodeEntityRepository ProductCodeEntityRepository => _productCodeEntityRepository ?? (_productCodeEntityRepository = new ProductCodeEntityRepository(_connection));
        public IProductCodeSheetUploadHistoryRepository ProductCodeSheetUploadHistoryRepository => _productCodeSheetUploadHistoryRepository ?? (_productCodeSheetUploadHistoryRepository = new ProductCodeSheetUploadHistoryRepository(_connection));
        public IProductCodeSheetUploadStatusRepository ProductCodeSheetUploadStatusRepository => _productCodeSheetUploadStatusRepository ?? (_productCodeSheetUploadStatusRepository = new ProductCodeSheetUploadStatusRepository(_connection));

        #endregion

        public IDbConnection Connection
        {
            get
            {
                if (_connection == null) _connection = new SqlConnection(_configuration.GetConnectionString("ITTConnectionString"));

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

        public void BeginTransaction()
        {
            try
            {
                if (_connection.State != ConnectionState.Open)
                    _connection.Open();

                _transaction = _connection.BeginTransaction();

                SetTransactions(_transaction);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SetTransactions(IDbTransaction transaction)
        {
            var repositories = GetType().GetProperties();

            foreach (var r in repositories)
            {
                if (Utility.IsAssignableToGenericType(r.PropertyType, typeof(IRepository<>)))
                {

                    var repo = r.GetValue(this) as IRepositoryTransaction;
                    repo.SetTransaction(transaction);

                }
            }
        }

        public void Commit()
        {
            try
            {
                if (_transaction != null)
                    _transaction.Commit();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public void Rollback()
        {
            try
            {
                if (_transaction != null)
                    _transaction.Rollback();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        #endregion

    }
}