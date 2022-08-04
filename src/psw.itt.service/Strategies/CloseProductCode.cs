using System;
using psw.itt.service.Command;
using PSW.ITT.Service.DTO;
using psw.itt.service.Exception;
using PSW.Lib.Logs;

namespace psw.itt.service.Strategies
{
    public class CloseProductCode : ApiStrategy<Unspecified, Unspecified>
    {
        #region Constructors
        public CloseProductCode(CommandRequest commandRequest) : base(commandRequest)
        {

        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);

                ResponseDTO = new Unspecified();
                // Prepare and return command reply
                return OKReply("Test Successful congratulations buddy :)");
            }
            catch (ServiceException ex)
            {
                Log.Error("|{0}|{1}| Service exception caught: {2}", StrategyName, MethodID, ex.Message);
                return InternalServerErrorReply(ex);
            }
            catch (System.Exception ex)
            {
                Log.Error("|{0}|{1}| System exception caught: {2}", StrategyName, MethodID, ex.Message);
                return InternalServerErrorReply(ex);
            }
        }
        #endregion
    }
}