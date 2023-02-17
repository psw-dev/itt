using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Command;
using System;
using System.Collections.Generic;
using PSW.Lib.Logs;
using System.Security.Claims;
using PSW.ITT.Service.ModelValidators;
using System.Linq;
using PSW.ITT.Data.Entities;
using PSW.ITT.Common.Constants;
using PSW.ITT.Data.DTO;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSW.ITT.Common.Model;

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

            //TOOD: Transform incoming TARP Request DTO to ITT Request DTO

            
            //Now use ITT Request DTO in your internal functions
            //TODO: Call Get Active Products code from ITT using ITT Request DTO
            //TODO: Call LPCO Regulation using Active Product code and ITT Request DTO
            //TODO: Get Factor List against Active Product Codes and Filter based on Factors code in ITT Request DTO
            //TODO: Filter Regulation based on Factor
            //TODO: Kharafat ki processing
            //TODO: Fill ITT Response DTO
            //TODO: Transform ITT Response DTO to TARP Response DTO
            //TODO: Return TARP Response DTO 

            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);

                //Check if the Product Code is available, active and regulated in the system
                ProductCodeWithAgencyLink tempHsCode = Command.UnitOfWork.ProductCodeEntityRepository.GetActiveProductCodeDetail(Convert.ToInt32(RequestDTO.AgencyId), (short)RequestDTO.TradeTranTypeID, RequestDTO.HsCode);

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
                
                factorDataList = lpcoRegulation.Select( x=> new LOVItem {ItemKey = x.FactorID.ToString(),ItemValue =x.Factor}).ToList();

                if (factorDataList == null)
                    {
                        return BadRequestReply("Factors data not found");
                    }

                Log.Information("|{0}|{1}| FactorData DbRecord {@factorDataList}", StrategyName, MethodID, factorDataList);

                //Get Regulation requested againt factor 
                var regulation = new LPCORegulation();

                if(RequestDTO.FactorCodeValuePair.Count>0){
                    regulation = lpcoRegulation.Where(x=>x.Factor.Equals(RequestDTO.FactorCodeValuePair.Values.FirstOrDefault().FactorValue)).FirstOrDefault();
                    if (regulation == null)
                    {
                        return BadRequestReply(String.Format("No record found for HsCode : {0}  Factor : {1}", RequestDTO.HsCode, RequestDTO.FactorCodeValuePair.Values.FirstOrDefault().FactorValue));
                    }
                }
                else{
                    regulation = lpcoRegulation.FirstOrDefault();
                    if (regulation == null)
                        {
                            return BadRequestReply(String.Format("No record found for HsCode : {0}", RequestDTO.HsCode));
                        }
                }
                
            JObject regulationJson = JObject.Parse(regulation.RegulationJson);
                var documentClassificationCode = "";
                if(RequestDTO.documentTypeCode==null)
                {
                    if(RequestDTO.TradeTranTypeID==(int)PSW.ITT.Common.Constants.TradeTranType.IMPORT){
                        documentClassificationCode = DocumentClassificationCode.RELEASE_ORDER;
                    }
                    else if(RequestDTO.TradeTranTypeID==(int)PSW.ITT.Common.Constants.TradeTranType.EXPORT){
                        documentClassificationCode = DocumentClassificationCode.EXPORT_CERTIFICATE;
                    }
                }
                else{
                    var docType = this.Command.SHRDUnitOfWork.DocumentTypeRepository.Where(new { Code = RequestDTO.documentTypeCode }).FirstOrDefault();
                    documentClassificationCode = docType.DocumentClassificationCode;
                }
                
                Log.Information("|{0}|{1}| Required LPCO Parent Code {@documentClassification}", StrategyName, MethodID, documentClassificationCode);


                
                ResponseDTO = new GetDocumentRequirementResponse();

                bool DocumentIsRequired = false;
                bool IsParenCodeValid = false;

                //Reterive information if LPCO require documentary requirement 
                // working of TARP mongoDBRecordFetcher.CheckIfLPCORequired
                DocumentIsRequired = CheckIfLPCORequired(regulationJson, documentClassificationCode, out IsParenCodeValid);

                if (!IsParenCodeValid)
                {
                    Log.Information("|{0}|{1}| Parent Code is Not Valid", StrategyName, MethodID);

                    return BadRequestReply("Document does not belong to a supported document Classification");
                }
                else if (!DocumentIsRequired)
                {
                    Log.Information("|{0}|{1}| LPCO required {2}", StrategyName, MethodID, "false");

                    ResponseDTO.isLPCORequired = false;

                    return OKReply(string.Format("{0} not required for HsCode : {1} and Factor : {2}", documentClassificationCode, RequestDTO.HsCode, RequestDTO.FactorCodeValuePair.Values.FirstOrDefault().FactorValue));
                }

                Log.Information("|{0}|{1}| LPCO required {2}", StrategyName, MethodID, "true");

                //Skiping CheckFactorInMongoRecord from TARP starategy as it is not required in ITT database structure

                var tempDocumentaryRequirementList = new List<DocumentaryRequirement>();
                
                //var response =
                 GetRequirements(regulationJson, documentClassificationCode);

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

        #region Methods
        public void GetRequirements(JObject mongoRecord, string documentClassification)
        {
            //SingleResponseModel<GetDocumentRequirementResponse>
             og.Information("[{0}.{1}] Started", GetType().Name, MethodBase.GetCurrentMethod().Name);
            GetDocumentRequirementResponse tarpRequirments = new GetDocumentRequirementResponse();
            var response = new SingleResponseModel<GetDocumentRequirementResponse>();

            var tarpDocumentRequirements = new List<DocumentaryRequirement>();
            var FinancialRequirement = new FinancialRequirement();
            var ValidityRequirement = new ValidityRequirement();

            tarpRequirments.isLPCORequired = true;

            if (documentClassification == DocumentClassificationCode.IMPORT_PERMIT || documentClassification == DocumentClassificationCode.PRODUCT_REGISTRATION)
            {
                var ipDocRequirements = new List<string>();
                var ipDocRequirementsTrimmed = new List<string>();
                var ipDocOptional = new List<string>();
                var ipDocOptionalTrimmed = new List<string>();

                if (RequestDTO.AgencyId == "4")
                {
                    ipDocRequirements = mongoRecord["ENLISTMENT OF SEED VARIETY MANDATORY DOCUMENTARY REQURIMENTS"].ToString().Split('|').ToList();
                    ipDocOptional = mongoRecord["ENLISTMENT OF SEED VARIETY OPTIONAL DOCUMENTARY REQURIMENTS"].ToString().Split('|').ToList();

                    //Financial Requirements
                    FinancialRequirement.PlainAmount = mongoRecord["ENLISTMENT OF SEED VARIETY  FEES"].ToString();
                    FinancialRequirement.Amount = Command.CryptoAlgorithm.Encrypt(mongoRecord["ENLISTMENT OF SEED VARIETY  FEES"].ToString());
                }
            }
        }
        public bool CheckIfLPCORequired(JObject mongoRecord, string requiredDocumentParentCode, out bool IsParenCodeValid)
        {
           
            switch (requiredDocumentParentCode)
            {
                case DocumentClassificationCode.IMPORT_PERMIT:
                    IsParenCodeValid = true;
                    return mongoRecord["ipRequired"].Value<string>().ToLower() == "yes";

                case DocumentClassificationCode.RELEASE_ORDER:
                    IsParenCodeValid = true;
                    return mongoRecord["roRequired"].Value<string>().ToLower() == "yes";

                case DocumentClassificationCode.EXPORT_CERTIFICATE:
                    IsParenCodeValid = true;
                   return mongoRecord["ecRequired"].Value<string>().ToLower() == "yes";

                case DocumentClassificationCode.PRODUCT_REGISTRATION:
                IsParenCodeValid = true;
                return mongoRecord["isProductRegistrationRequired"].ToString().ToLower() == "yes";


                default:
                    IsParenCodeValid = false;
                    return false;
            }
        }
        #endregion
    }
}
