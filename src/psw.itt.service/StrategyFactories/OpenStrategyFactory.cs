

using psw.itt.data;
using psw.itt.service.Command;

namespace psw.itt.service.Strategies
{
    public class OpenStrategyFactory : IStrategyFactory
    {
        #region Private Variables

        #endregion

        #region Properties
        public IUnitOfWork UnitOfWork { get; protected set; }

        #endregion

        #region Constructor

        public OpenStrategyFactory(IUnitOfWork uow)
        {
            UnitOfWork = uow;
        }

        #endregion

        #region Private Method

        #endregion

        #region Public Methods

        public Strategy CreateStrategy(CommandRequest request)
        {

            switch (request.methodId)
            {
                default: break;
            }

            return new InvalidStrategy(request);
        }

        // s

        #endregion

    }

}
