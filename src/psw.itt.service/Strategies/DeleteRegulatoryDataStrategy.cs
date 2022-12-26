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
                var RegulationEntity = Command.UnitOfWork.LPCORegulationRepository.Get(RequestDTO.ID);
                if (RequestDTO.Immediately)
                {
                    Command.UnitOfWork.BeginTransaction();
                    RegulationEntity.EffectiveThruDt = currentDateTime;
                    RegulationEntity.UpdatedBy = Command.LoggedInUserRoleID;
                    RegulationEntity.UpdatedOn = currentDateTime;
                    Command.UnitOfWork.LPCORegulationRepository.Update(RegulationEntity);
                    Command.UnitOfWork.Commit();
                }
                else
                {
                    Command.UnitOfWork.BeginTransaction();
                    RegulationEntity.EffectiveThruDt = RequestDTO.EndDate;
                    RegulationEntity.UpdatedBy = Command.LoggedInUserRoleID;
                    RegulationEntity.UpdatedOn = currentDateTime;
                    Command.UnitOfWork.LPCORegulationRepository.Update(RegulationEntity);
                    Command.UnitOfWork.Commit();
                }

                // Prepare and return command reply
                return OKReply("Regulation Deleted Successfully");
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