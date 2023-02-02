
using PSW.ITT.Data;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.Strategies;

namespace PSW.ITT.Service.Strategies
{
    public class IntegrationStrategyFactory : IStrategyFactory
    {
        #region Private Variables

        #endregion

        #region Properties
        public IUnitOfWork UnitOfWork { get; protected set; }
        public ISHRDUnitOfWork SHRDUnitOfWork { get; protected set; }

        #endregion

        #region Constructor

        public IntegrationStrategyFactory(IUnitOfWork uow, ISHRDUnitOfWork shrdUow)
        {
            UnitOfWork = uow;
            SHRDUnitOfWork = shrdUow;
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
