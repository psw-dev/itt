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
    public class ValidateRegulatedHSCodesStrategy : ApiStrategy<ValidateRegulatedHSCodesRequestDTO, ValidateRegulatedHSCodesResponseDTO>
    {
        #region Constructors 
        public ValidateRegulatedHSCodesStrategy(CommandRequest request) : base(request)
        {
            Reply = new CommandReply();
            this.Validator = new ValidateRegulatedHSCodesRequestDTOValidator();
        }
        #endregion 

        #region Distructors 
        ~ValidateRegulatedHSCodesStrategy()
        {

        }
        #endregion 

        #region Strategy Excecution  

        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);

                var RegulatedDPPImportHSCodes = Command.UnitOfWork.ProductCodeAgencyLinkRepository.ValidateRegulatedHSCodes(RequestDTO.HSCodes, RequestDTO.AgencyID, RequestDTO.TradeTranTypeId);
                var responseList = new List<string>();
                
                foreach(var x in RegulatedDPPImportHSCodes)
                {
                    var lpcoRegulation = Command.UnitOfWork.LPCORegulationRepository.GetRegulationByProductAgencyLinkID(x.Item2).FirstOrDefault();
                    JObject regulationJson = JObject.Parse(lpcoRegulation.RegulationJson);
                     if(regulationJson.ContainsKey("psiRequired")){
                        var PSIRequired = getLowerValue(regulationJson["psiRequired"]);
                        if(PSIRequired!=null)
                        {
                            responseList.Add(x.Item1);
                        }    
                    }
                }

                ResponseDTO = new ValidateRegulatedHSCodesResponseDTO{
                    HSCodes= responseList
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
        private string getLowerValue(JToken str){
            return str.Value<string>().ToLower();
        }
        #endregion 

    }
}
