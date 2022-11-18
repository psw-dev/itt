using System;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using PSW.ITT.Data.Entities;
using System.Text.Json;

namespace PSW.ITT.Service.Strategies
{
    public class UpdateSingleRegulationStrategy : ApiStrategy<UpdateSingleRegulationRequestDTO, Unspecified>
    {
        private DateTime currentDateTime = DateTime.Now;
        #region Constructors
        public UpdateSingleRegulationStrategy(CommandRequest commandRequest) : base(commandRequest)
        {
            // this.Validator = new UploadSingleProductCodeRequestDTOValidator();
        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
                Command.UnitOfWork.BeginTransaction();
                var regulation = Command.UnitOfWork.LPCORegulationRepository.Get(RequestDTO.ID);
                regulation.RegulationJson = JsonSerializer.Serialize<dynamic>(RequestDTO.Data);
                regulation.UpdatedOn = currentDateTime;
                regulation.UpdatedBy = Command.LoggedInUserRoleID;
                // var LPCOid = 
                Command.UnitOfWork.LPCORegulationRepository.Update(regulation);

                // var regulationAgencyLink = new ProductRegulationRequirement();
                // regulationAgencyLink.ProductCodeAgencyLinkID = RequestDTO.ProductCodeAgencyLinkID;
                // regulationAgencyLink.LPCOFeeStructureID = 3; //TODO: remove hardcoded value
                // regulationAgencyLink.LPCORegulationID = LPCOid;
                // regulationAgencyLink.EffectiveFromDt = currentDateTime;
                // regulationAgencyLink.EffectiveThruDt = new DateTime(9999, 12, 30);
                // regulationAgencyLink.CreatedOn = currentDateTime;
                // regulationAgencyLink.UpdatedOn = currentDateTime;
                // regulationAgencyLink.CreatedBy = Command.LoggedInUserRoleID;
                // regulationAgencyLink.UpdatedBy = Command.LoggedInUserRoleID;
                // regulationAgencyLink.TradeTranTypeID = RequestDTO.TradeTranTypeID;
                // Command.UnitOfWork.ProductRegulationRequirementRepository.Add(regulationAgencyLink);
                Command.UnitOfWork.Commit();
                // Prepare and return command reply
                return OKReply("Regulation Uploaded Successfully");
            }
            catch (ServiceException ex)
            {
                Log.Error("|{0}|{1}| Service exception caught: {2}", StrategyName, MethodID, ex.Message);
                Command.UnitOfWork.Rollback();
                return InternalServerErrorReply(ex);
            }
            catch (System.Exception ex)
            {
                Log.Error("|{0}|{1}| System exception caught: {2}", StrategyName, MethodID, ex.Message);
                Command.UnitOfWork.Rollback();
                return InternalServerErrorReply(ex);
            }
        }
        #endregion
    }
}