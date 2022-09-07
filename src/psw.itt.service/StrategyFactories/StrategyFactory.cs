using PSW.ITT.Data;
using PSW.ITT.Service.Command;

namespace PSW.ITT.Service.Strategies
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
                case "2200": return new FetchActiveProductCodesListStrategy(request);
                case "2201": return new UploadSingleProductCodeStrategy(request);
                case "2202": return new ExtendProductCodeStrategy(request);
                case "2203": return new CloseProductCodeStrategy(request);
                case "2204": return new GetProductCodeListWithAgenciesStrategy(request);
                case "2205": return new UpdateProductCodeAgencyAssociationStrategy(request);
                case "2206": return new GetAgencyList(request);
                case "2220": return new GetColumnNamesForExcelStrategy(request);
                case "2221": return new FetchUploadedSheetsListStrategy(request);
                case "2222": return new UploadFileStrategy(request);
                case "2223": return new GetUploadFileProgressStrategy(request);
                case "2224": return new UpdateFileHistoryStatusStrategy(request);

                default: break;
            }

            return new InvalidStrategy(request);
        }
        #endregion
    }
}