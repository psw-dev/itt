using PSW.RabbitMq;
using System;

namespace psw.itt.data
{
    public interface IUnitOfWork : IDisposable
    {

        #region Repositories
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
