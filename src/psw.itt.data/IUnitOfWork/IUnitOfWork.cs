using psw.itt.data.IRepositories;
using PSW.RabbitMq;
using System;

namespace psw.itt.data
{
    public interface IUnitOfWork : IDisposable
    {

        #region Repositories
        IChapterAgencyLinkRepository ChapterAgencyLinkRepository { get; }
        IProductCodeChapterRepository ProductCodeChapterRepository { get; }
        IProductCodeEntityRepository ProductCodeEntityRepository { get; }
        IProductCodeSheetUploadHistoryRepository ProductCodeSheetUploadHistoryRepository { get; }
        IProductCodeSheetUploadStatusRepository ProductCodeSheetUploadStatusRepository { get; }

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
