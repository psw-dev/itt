using System;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using System.Linq;
using PSW.ITT.Service.ModelValidators;
using PSW.ITT.Service.BusinessLogicLayer;

namespace PSW.ITT.Service.Strategies
{
    public class ExtendProductCodeStrategy : ApiStrategy<EditProductCodeRequestDTO, Unspecified>
    {
        private DateTime currentDateTime = DateTime.Now;
        #region Constructors
        public ExtendProductCodeStrategy(CommandRequest commandRequest) : base(commandRequest)
        {
            this.Validator = new ExtendProductCodeRequestDTOValidator();
        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
                var ProductCodeEntity = Command.UnitOfWork.ProductCodeEntityRepository.Get(RequestDTO.ID);
                if (ProductCodeEntity.EffectiveThruDt.Date == currentDateTime.Date)
                {
                    return BadRequestReply("Product Code already deactivated");
                }
                if (DateTime.Compare(ProductCodeEntity.EffectiveThruDt, RequestDTO.EffectiveThruDt) < 0)
                {
                    Log.Error($"|ProductCodeValidation| Product code end date can not be set before start date");
                    return BadRequestReply($"the extended effective thru {RequestDTO.EffectiveThruDt} should always greater than Old Effective Thru date{ProductCodeEntity.EffectiveThruDt}");
                }
                ProductCodeEntity.EffectiveThruDt = RequestDTO.EffectiveThruDt;
                ProductCodeEntity.UpdatedBy = Command.LoggedInUserRoleID;
                ProductCodeEntity.UpdatedOn = currentDateTime;

                Command.UnitOfWork.ProductCodeEntityRepository.Update(ProductCodeEntity);


                // Prepare and return command reply
                return OKReply("Product Code Update Sucessfully");
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