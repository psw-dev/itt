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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PSW.ITT.Service.Strategies
{
    public class GetRegulatedHSCodeListStrategy : ApiStrategy<GetRegulatedHscodeListRequest, GetRegulatedHscodeListResponse>
    {
        #region Constructors 
        public GetRegulatedHSCodeListStrategy(CommandRequest request) : base(request)
        {
            Reply = new CommandReply();
            // this.Validator = new GetRegulatedHscodeListRequestDTOValidator();
        }
        #endregion 

        #region Distructors 
        ~GetRegulatedHSCodeListStrategy()
        {

        }
        #endregion 

        #region Strategy Excecution  

        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);

                var regulatedHSCodeList = new List<ViewRegulatedHsCode>();

                //Get Regulated Hscode list filtered on base of AgencyId 
                if (RequestDTO.AgencyId != 0 && RequestDTO.TradeTranTypeID == null)
                {
                   regulatedHSCodeList = Command.UnitOfWork.ProductCodeAgencyLinkRepository.GetRegulatedHsCodeList(RequestDTO.AgencyId);
                    if (regulatedHSCodeList == null || regulatedHSCodeList.Count == 0)
                    {
                        return BadRequestReply("Hscodes not available against provided Agency");
                    }
                }

                //Get Regulated Hscode list filtered on base of AgencyId and DocumentTypeCode
                else if (RequestDTO.AgencyId != 0 && RequestDTO.TradeTranTypeID != null)
                {
                    regulatedHSCodeList = Command.UnitOfWork.ProductCodeAgencyLinkRepository.GetRegulatedHsCodeList(RequestDTO.AgencyId, RequestDTO.TradeTranTypeID);
                    if (regulatedHSCodeList == null || regulatedHSCodeList.Count == 0)
                    {
                        return BadRequestReply("Hscodes not available against provided Agency and Document");
                    }
                }

                else
                {
                    regulatedHSCodeList = Command.UnitOfWork.ProductCodeAgencyLinkRepository.GetRegulatedHsCodeList();
                    
                    if (regulatedHSCodeList == null || regulatedHSCodeList.Count == 0)
                    {
                        return BadRequestReply("Hscodes not available");
                    }
                }

                //Get hscodeDetails
                foreach (var regulatedHscode in regulatedHSCodeList)
                {
                    regulatedHscode.HsCodeDetailsList = Command.UnitOfWork.ProductCodeAgencyLinkRepository.GetRegulatedHsCodeList(RequestDTO.TradeTranTypeID, RequestDTO.AgencyId, regulatedHscode.HsCode);
                    foreach (var hsCodeDetails in regulatedHscode.HsCodeDetailsList)
                    {       
                    var lpcoRegulation = Command.UnitOfWork.LPCORegulationRepository.GetRegulationByProductAgencyLinkID(hsCodeDetails.ProductCodeAgencyLinkID).FirstOrDefault();
                    JObject regulationJson = JObject.Parse(lpcoRegulation.RegulationJson);
                    hsCodeDetails.TechnicalName = getValue(regulationJson["technicalName"]);
                    }
                }

                ResponseDTO = new GetRegulatedHscodeListResponse
                {
                    RegulatedHsCodeList = regulatedHSCodeList
                };


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
        
        private string getValue(JToken str){
            return str.Value<string>();
        }
        #endregion 

    }
}
