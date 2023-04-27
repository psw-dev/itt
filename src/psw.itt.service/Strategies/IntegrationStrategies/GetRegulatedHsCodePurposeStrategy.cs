using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Command;
using System;
using System.Collections.Generic;
using PSW.Lib.Logs;
using System.Security.Claims;
using PSW.ITT.Service.ModelValidators;
using System.Linq;
using PSW.ITT.Data.Entities;
using PSW.ITT.Common.Enums;

namespace PSW.ITT.Service.Strategies
{
    public class GetRegulatedHsCodePurposeStrategy : ApiStrategy<RegulatedHsCodePurposeRequestDTO, RegulatedHsCodePurposeResponseDTO>
    {
        #region Constructors 
        public GetRegulatedHsCodePurposeStrategy(CommandRequest request) : base(request)
        {
            Reply = new CommandReply();
            // this.Validator = new GetPCTCodeListRequestDTOValidator();
        }
        #endregion 

        #region Distructors 
        ~GetRegulatedHsCodePurposeStrategy()
        {

        }
        #endregion 

        #region Strategy Excecution  

        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
        
                var agencyAssociatedHscodeList = Command.UnitOfWork.ProductCodeAgencyLinkRepository.GetAgencyAssociatedHsCodeList(RequestDTO.AgencyId, RequestDTO.TradeTranTypeID);

                if (agencyAssociatedHscodeList == null || agencyAssociatedHscodeList.Count == 0)
                {
                    return BadRequestReply("Hs Codes does not exist against this Agency.");
                }
                ResponseDTO = new RegulatedHsCodePurposeResponseDTO();
                var regulatedHsCodePurposeList = new List<RegulatedHsCodePurpose>();
                foreach (var agencyAssociatedHscode in agencyAssociatedHscodeList){
                    var lpcoRegulationList = Command.UnitOfWork.LPCORegulationRepository.GetRegulationByProductAgencyLinkID(agencyAssociatedHscode.ID).ToList();
                    var regulatedHsCodePurpose =  new RegulatedHsCodePurpose();
                    regulatedHsCodePurpose.HsCode = lpcoRegulationList.FirstOrDefault().HSCodeExt;
                    regulatedHsCodePurpose.PurposeList = new List<string>();
                    foreach(var lpcoRegulation in lpcoRegulationList){
                        regulatedHsCodePurpose.PurposeList.Add(lpcoRegulation.Factor);
                    }
                    regulatedHsCodePurposeList.Add(regulatedHsCodePurpose);
                }
                ResponseDTO.RegulatedHsCodePurposeList=regulatedHsCodePurposeList;
                    

                //     Log.Information("|{0}|{1}| Response DTO : {@ResponseDTO}", StrategyName, MethodID, ResponseDTO);

                //     return OKReply();
                

                // ResponseDTO = new GetPCTCodeListResponse
                // {
                //     Message = "Product codes exist for provided hscode.",
                //     PctCodeList = tempPctCodeList
                // };

                Log.Information("|{0}|{1}| Response DTO : {@ResponseDTO}", StrategyName, MethodID, ResponseDTO);

                // Send Command Reply 
                return OKReply();
            }
            catch (System.Exception ex)
            {
                Log.Error("|{0}|{1}| Exception Occurred {@ex}", StrategyName, MethodID, ex);
                return InternalServerErrorReply(ex);
            }
        }
        #endregion 

    }
}
