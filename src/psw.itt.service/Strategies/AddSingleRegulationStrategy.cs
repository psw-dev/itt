using System;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using PSW.ITT.Data.Entities;
using System.Text.Json;
using System.Linq;

namespace PSW.ITT.Service.Strategies
{
    public class AddSingleRegulationStrategy : ApiStrategy<AddSingleRegulationRequestDTO, Unspecified>
    {
        private DateTime currentDateTime = DateTime.Now;
        #region Constructors
        public AddSingleRegulationStrategy(CommandRequest commandRequest) : base(commandRequest)
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
                var productcode = RequestDTO.Data["hsCode"].ToString() + "." + RequestDTO.Data["productCode"].ToString();
                var duplicateCheckList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, 1).Where(x => x.CheckDuplicate == true).ToList();
                duplicateCheckList.RemoveAll(x => x.NameLong.Contains("HSCode"));
                duplicateCheckList.RemoveAll(x => x.NameLong.Contains("Product Code"));
                var factor = RequestDTO.Data[duplicateCheckList[0].NameShort];
                RequestDTO.Data["productCode"] = productcode;
                Command.UnitOfWork.BeginTransaction();
                var regulation = new LPCORegulation();
                regulation.AgencyID = RequestDTO.AgencyID;
                regulation.RegulationJson = JsonSerializer.Serialize<dynamic>(RequestDTO.Data);
                regulation.CreatedOn = currentDateTime;
                regulation.UpdatedOn = currentDateTime;
                regulation.CreatedBy = Command.LoggedInUserRoleID;
                regulation.UpdatedBy = Command.LoggedInUserRoleID;
                regulation.HsCode = RequestDTO.Data["hsCode"].ToString();
                regulation.HsCodeExt = RequestDTO.Data["productCode"].ToString();
                regulation.ProductCodeAgencyLinkID = RequestDTO.ProductCodeAgencyLinkID;
                regulation.Factor = factor;
                // regulation.EffectiveFromDt = currentDateTime;
                // regulation.EffectiveThruDt = new DateTime(9999, 12, 30);
                regulation.TradeTranTypeID = RequestDTO.TradeTranTypeID;
                var LPCOid = Command.UnitOfWork.LPCORegulationRepository.Add(regulation);

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