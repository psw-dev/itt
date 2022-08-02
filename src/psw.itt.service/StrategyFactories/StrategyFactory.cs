using psw.itt.data;
using psw.itt.service.Command;

namespace psw.itt.service.Strategies
{
    public class StrategyFactory : IStrategyFactory
    {
        #region Private Variables

        #endregion

        #region Properties
        public IUnitOfWork UnitOfWork { get; protected set; }
        #endregion

        #region Constructor & Destructor
        public StrategyFactory(IUnitOfWork uow)
        {
            UnitOfWork = uow;
        }
        #endregion

        #region Public Methods
        public virtual Strategy CreateStrategy(CommandRequest request)
        {
            switch (request.methodId)
            {
                case "2200": return new TestStrategy(request);
                default: break;
            }

            return new InvalidStrategy(request);
        }
        #endregion
    }
}