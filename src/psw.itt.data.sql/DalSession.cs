using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;

// using PSW.ITT.Data;

namespace PSW.ITT.Data.Sql
{
    public sealed class DalSession : IDalSession, IDisposable
    {
        SqlConnection _connection = null;
        UnitOfWork.UnitOfWork _unitOfWork = null;

        public DalSession(IConfiguration _configuration)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            //_unitOfWork = new UnitOfWork(_connection);
        }

        public DalSession(string _connectionString)
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
            //_unitOfWork = new UnitOfWork(_connection);
        }

        public UnitOfWork.UnitOfWork UnitOfWork
        {
            get { return _unitOfWork; }
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
            _connection.Dispose();
        }
    }
}