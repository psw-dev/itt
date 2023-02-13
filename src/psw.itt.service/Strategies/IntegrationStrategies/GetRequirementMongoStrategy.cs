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
    public class GetRequirementMongoStrategy : ApiStrategy<GetDocumentRequirementRequest, GetDocumentRequirementResponse>
    {
        #region Constructors 
        public GetRequirementMongoStrategy(CommandRequest request) : base(request)
        {
            Reply = new CommandReply();
            this.Validator = new GetRequirementMongoRequestDTOValidator();
        }
        #endregion 

        #region Distructors 
        ~GetRequirementMongoStrategy()
        {

        }
        #endregion 

        #region Strategy Excecution  

        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);

                //Check if the Product Code is available, active and regulated in the system
                ProductCodeWithAgencyLink tempHsCode = Command.UnitOfWork.ProductCodeEntityRepository.GetActiveProductCodeDetail(RequestDTO.AgencyId, RequestDTO.TradeTranTypeID, RequestDTO.HsCode);

                Log.Information("|{0}|{1}| ProductCodeEntity DbRecord {@tempHsCode}", StrategyName, MethodID, tempHsCode);

                if (tempHsCode == null)
                {
                    return BadRequestReply("Record for hscode does not exist");
                }
                //Not involving rule implementation as of TARP

                //Get regulation against hscodeExt
                var lpcoRegulation = new  List<LPCORegulation>();
                
                lpcoRegulation = Command.UnitOfWork.LPCORegulationRepository.GetRegulationByProductAgencyLinkID(tempHsCode.ProductCodeAgencyLinkID);

                    if (lpcoRegulation == null)
                    {
                        return BadRequestReply("Regulation data not found");
                    }

                Log.Information("|{0}|{1}| Regulation DbRecord {@lpcoRegulation}", StrategyName, MethodID, lpcoRegulation);
                
                //Get Factor Lsit
                var factorDataList = new  List<LOVItem>();
                
                factorDataList = lpcoRegulation.Select(x=>x.FactorID ,x.Factor).ToList();

                if (factorDataList == null)
                    {
                        return BadRequestReply("Factors data not found");
                    }

                Log.Information("|{0}|{1}| FactorData DbRecord {@factorDataList}", StrategyName, MethodID, factorDataList);

                //Get Regulation requested againt factor 
                var regulation = new LPCORegulation();

                if(RequestDTO.FactorCodeValuePair.Count>0){
                    regulation = lpcoRegulation.Where(x=>x.FactorID=RequestDTO.FactorCodeValuePair[0].FactorValueID).FirstOrDefault();
                    if (regulation == null)
                    {
                        return BadRequestReply(String.Format("No record found for HsCode : {0}  Factor : {1}", RequestDTO.HsCode, RequestDTO.FactorCodeValuePair[0].FactorValue));
                    }
                }
                else{
                    regulation = lpcoRegulation.FirstOrDefault();
                    if (regulation == null)
                        {
                            return BadRequestReply(String.Format("No record found for HsCode : {0}", RequestDTO.HsCode));
                        }
                }

                var docType = this.Command.UnitOfWork.DocumentTypeRepository.Where(new { Code = RequestDTO.documentTypeCode }).FirstOrDefault();

                Log.Information("|{0}|{1}| Required LPCO Parent Code {@documentClassification}", StrategyName, MethodID, docType.DocumentClassificationCode);




                ResponseDTO = new GetPCTCodeListResponse
                {
                    Message = "Product codes exist for provided hscode.",
                    PctCodeList = tempPctCodeList
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
        #endregion 
        
        #region Methods
        public bool CheckIfLPCORequired(string mongoRecord, string requiredDocumentParentCode, out bool IsParenCodeValid)
        {
            containerObject = new JSONObject(mongoRecord);
            switch (requiredDocumentParentCode)
            {
                case DocumentClassificationCode.IMPORT_PERMIT:
                    IsParenCodeValid = true;
                    return containerObject.optString("ipRequired").ToString().ToLower() == "yes";

                case DocumentClassificationCode.RELEASE_ORDER:
                    IsParenCodeValid = true;
                    return containerObject.optString("roRequired").ToString().ToLower() == "yes";

                case DocumentClassificationCode.EXPORT_CERTIFICATE:
                    IsParenCodeValid = true;
                   return containerObject.optString("ecRequired").ToString().ToLower() == "yes";

                default:
                    IsParenCodeValid = false;
                    return false;
            }
        }
        #endregion
    }
}
