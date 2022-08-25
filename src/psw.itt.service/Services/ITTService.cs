using PSW.ITT.Data;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.Exception;
using PSW.ITT.Service.Strategies;
using PSW.Common.Crypto;
using psw.itt.service;
using PSW.ITT.Service.IServices;
using System.Collections.Generic;
using System.Security.Claims;

namespace PSW.ITT.Service.Services
{
    public class ITTService : IITTService
    {
        #region properties 
        public IUnitOfWork UnitOfWork { get; set; }
        public IStrategyFactory StrategyFactory { get; set; }
        public ICryptoAlgorithm CryptoAlgorithm { get; set; }
        public IEnumerable<Claim> UserClaims { get; set; }
        public int LoggedInUserRoleId { get; set; }

        #endregion

        #region constuctors & destroctors

        public ITTService()
        {
        }

        #endregion

        #region Invoke Function 

        public CommandReply invokeMethod(CommandRequest request)
        {
            try
            {
                //check if UnitOfWork is set otherwise set the service's UoW as default
                request.UnitOfWork = request.UnitOfWork ?? UnitOfWork;

                request.UserClaims = request.UserClaims ?? this.UserClaims;
                // Check if CryptoAlgorith is set otherwise set the service's Crypto Algorithm as default
                request.CryptoAlgorithm = request.CryptoAlgorithm ?? CryptoAlgorithm;
                //create strategy based on request. it can be dynamic
                var strategy = StrategyFactory.CreateStrategy(request);
                //validate request for strategy
                var isValide = strategy.Validate();
                //Execute strategy
                var reply = strategy.Execute();
                return reply;
            }
            catch (ServiceException ex)
            {
                //TODO: Catch the exception 
                //_logger.Log("Error Occured : " + ex.Message);
                throw ex;
            }
        }

        #endregion
    }
}