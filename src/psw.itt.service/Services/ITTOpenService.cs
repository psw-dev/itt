
using PSW.Common.Crypto;
using PSW.ITT.Data;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.IServices;
using PSW.ITT.Service.Strategies;

namespace PSW.ITT.Service.Services
{
    public class ITTOpenService : IITTOpenService
    {
        #region properties 
        public IUnitOfWork UnitOfWork { get; set; }
        public ISHRDUnitOfWork SHRDUnitOfWork { get; set; }
        public IStrategyFactory StrategyFactory { get; set; }
        public ICryptoAlgorithm CryptoAlgorithm { get; set; }
        public int LoggedInUserRoleId { get; set; }
        #endregion


        #region constuctors & destroctors
        
        public ITTOpenService()
        {
        }

        #endregion

        #region Invoke Function 

        /// <summary>
        /// Invoke method for RPC Server
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public CommandReply Invoke(CommandRequest request)
        {
            try
            {
                // Initialize Mapper in Command Request 
                // request._mapper = this._mapper;
                //check if UnitOfWork is set otherwise set the service's UoW as default
                request.UnitOfWork = request.UnitOfWork ?? this.UnitOfWork;

                request.SHRDUnitOfWork = request.SHRDUnitOfWork ?? this.SHRDUnitOfWork;
                // Check if CryptoAlgorith is set otherwise set the service's Crypto Algorithm as default
                //request.CryptoAlgorithm = request.CryptoAlgorithm ?? this.CryptoAlgorithm;
                //create strategy based on request. it can be dynamic
                Strategy strategy = this.StrategyFactory.CreateStrategy(request);
                //validate request for strategy
                bool isValid = strategy.Validate();
                //Execute strategy
                var reply = isValid
                    ? strategy.Execute()
                    : strategy.BadRequestReply(strategy.ValidationMessage);
                return reply;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                request?.UnitOfWork?.CloseConnection();
            }
        }
        public CommandReply invokeMethod(CommandRequest request)
        {
            try
            {
                // Initialize Mapper in Command Request 
                // request._mapper = this._mapper;
                //check if UnitOfWork is set otherwise set the service's UoW as default
                request.UnitOfWork = request.UnitOfWork ?? this.UnitOfWork;
                // Check if CryptoAlgorith is set otherwise set the service's Crypto Algorithm as default
                request.CryptoAlgorithm = request.CryptoAlgorithm ?? this.CryptoAlgorithm;
                //create strategy based on request. it can be dynamic
                Strategy strategy = this.StrategyFactory.CreateStrategy(request);
                //validate request for strategy
                bool isValid = strategy.Validate();
                //Execute strategy
                var reply = isValid
                    ? strategy.Execute()
                    : strategy.BadRequestReply(strategy.ValidationMessage);
                return reply;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                request?.UnitOfWork?.CloseConnection();
            }
        }

        #endregion


    }

}