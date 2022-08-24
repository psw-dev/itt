using System;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using System.Linq;
using PSW.ITT.Service.ModelValidators;

namespace PSW.ITT.Service.Strategies
{
    public class CloseProductCodeStrategy : ApiStrategy<CloseProductCodeRequestDTO, Unspecified>
    {
        private DateTime currentDateTime = DateTime.Now;
        #region Constructors
        public CloseProductCodeStrategy(CommandRequest commandRequest) : base(commandRequest)
        {
            this.Validator = new CloseProductCodeRequestDTOValidator();
        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
                var ProductCodeEntity = Command.UnitOfWork.ProductCodeEntityRepository.Where(new
                {
                    HSCode = RequestDTO.HSCode,
                    ProductCode = RequestDTO.ProductCode
                }).FirstOrDefault();
                if (ProductCodeEntity.EffectiveThruDt <= currentDateTime)
                {
                    return BadRequestReply("Product Code already deactivated");
                }
                ProductCodeEntity.EffectiveThruDt = currentDateTime.AddDays(1).Date.AddSeconds(-1);
                ProductCodeEntity.UpdatedBy = Command.LoggedInUserRoleID;
                ProductCodeEntity.UpdatedOn = currentDateTime;
                // This will set time at max of same day
                // as Product code will be closed at the end of day 
                Command.UnitOfWork.ProductCodeEntityRepository.Update(ProductCodeEntity);

                // Prepare and return command reply
                return OKReply("Product Code Closed Successfully");
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