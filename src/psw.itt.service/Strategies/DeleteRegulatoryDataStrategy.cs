using System;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using System.Linq;
using PSW.ITT.Service.ModelValidators;

namespace PSW.ITT.Service.Strategies
{
    public class DeleteRegulatoryDataStrategy : ApiStrategy<DeleteRegulatoryDataRequestDTO, Unspecified>
    {
        private DateTime currentDateTime = DateTime.Now;
        #region Constructors
        public DeleteRegulatoryDataStrategy(CommandRequest commandRequest) : base(commandRequest)
        {
            this.Validator = new DeleteRegulatoryDataRequestDTOValidator();
        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
                var ProductRegulationEntity = Command.UnitOfWork.ProductRegulationRequirementRepository.Get(RequestDTO.ID);
                if (ProductRegulationEntity.EffectiveThruDt <= currentDateTime)
                {
                    return BadRequestReply("Product Code already deactivated");
                }
                Command.UnitOfWork.BeginTransaction();
                ProductRegulationEntity.EffectiveThruDt = currentDateTime;
                ProductRegulationEntity.UpdatedBy = Command.LoggedInUserRoleID;
                ProductRegulationEntity.UpdatedOn = currentDateTime;
                Command.UnitOfWork.ProductRegulationRequirementRepository.Update(ProductRegulationEntity);
                Command.UnitOfWork.Commit();
                // Prepare and return command reply
                return OKReply("Product Code Closed Successfully");
            }
            catch (ServiceException ex)
            {
                Log.Error("|{0}|{1}| Service exception caught: {2}", StrategyName, MethodID, ex.Message);
                Command.UnitOfWork.Rollback();
                return InternalServerErrorReply(ex);
            }
            catch (System.Exception ex)
            {
                Command.UnitOfWork.Rollback();
                Log.Error("|{0}|{1}| System exception caught: {2}", StrategyName, MethodID, ex.Message);
                return InternalServerErrorReply(ex);
            }
        }
        #endregion
    }
}