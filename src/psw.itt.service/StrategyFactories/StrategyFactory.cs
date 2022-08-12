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
                case "2200": return new FetchActiveProductCodesList(request);
                case "2201": return new UploadSingleProductCode(request);
                case "2202": return new EditProductCode(request);
                case "2203": return new CloseProductCode(request);
                case "2204": return new GetChaptersListWithAgencies(request);
                case "2205": return new UpdateChapterAgencyAssociation(request);
                case "2206": return new GetAgencyList(request);
                case "2220": return new GetColumnNamesForExcelStrategy(request);
                case "2221": return new FetchUploadedSheetsListStrategy(request);

                default: break;
            }

            return new InvalidStrategy(request);
        }
        #endregion
    }
}