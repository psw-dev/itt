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
using PSW.ITT.Common.Constants;
using PSW.ITT.Data.DTO;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSW.ITT.Common.Model;
using PSW.ITT.service;

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
                
                var response = GetRequirements(regulationJson, documentClassificationCode, regulation);

                if (!response.IsError)
                {
                    ResponseDTO = response.Model;
                }
                else
                {
                    Log.Error("|{0}|{1}| Error ", StrategyName, MethodID, response.Error.InternalError.Message);
                    throw new ArgumentException(response.Error.InternalError.Message);
                }
                ResponseDTO.FormNumber = GetFormNumber(regulationJson, documentClassificationCode);
               
                Log.Information("|{0}|{1}| Documentary Requirements {@tempDocumentaryRequirementList}", StrategyName, MethodID, tempDocumentaryRequirementList);

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
         public string GetFormNumber(JObject mongoRecord, string requiredDocumentParentCode)
        {
            switch (requiredDocumentParentCode)
            {
                case DocumentClassificationCode.IMPORT_PERMIT:
                    return getValue(mongoRecord["ipCertificateFormNumber"]);

                case DocumentClassificationCode.RELEASE_ORDER:
                    return getValue(mongoRecord["roCertificateFormNumber"]);

                case DocumentClassificationCode.EXPORT_CERTIFICATE:
                    return getValue(mongoRecord["ecCertificateFormNumber"]);

                case DocumentClassificationCode.PRODUCT_REGISTRATION:
                    return getValue(mongoRecord["prdCertificateFormNumber"]);

                case DocumentClassificationCode.PREMISE_REGISTRATION:
                    return getValue(mongoRecord["prmCertificateFormNumber"]);
                
            }
            return "";
        }
        public SingleResponseModel<GetDocumentRequirementResponse> GetRequirements(JObject mongoRecord, string documentClassification, LPCORegulation lpcoRegulation )
        {
            //SingleResponseModel<GetDocumentRequirementResponse>
            //  og.Information("[{0}.{1}] Started", GetType().Name, MethodBase.GetCurrentMethod().Name);
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

                if (Convert.ToInt32(RequestDTO.AgencyId) == (int)AgencyEnum.FSCRD)
                {
                    ipDocRequirements =getListValue(mongoRecord["prdMandatoryDocumentryRequirements"]);
                    ipDocOptional =getListValue( mongoRecord["prdOptionalDocumentryRequirements"]);

                    //Financial Requirements
                    FinancialRequirement.PlainAmount = getValue(mongoRecord["prdFees"]);
                    FinancialRequirement.Amount = Command.CryptoAlgorithm.Encrypt(getValue(mongoRecord["prdFees"]));
                }
                else{
                    ipDocRequirements = getListValue(mongoRecord["ipMandatoryDocumentryRequirements"]);
                    ipDocOptional = getListValue(mongoRecord["ipOptionalDocumentryRequirements"]);

                    //Financial Requirements
                    FinancialRequirement.PlainAmount = getValue(mongoRecord["ipFees"]);
                    FinancialRequirement.Amount = Command.CryptoAlgorithm.Encrypt( getValue(mongoRecord["ipFees"]));
                    FinancialRequirement.PlainAmmendmentFee =  getValue(mongoRecord["ipAmendmentFees"]);
                    FinancialRequirement.AmmendmentFee = Command.CryptoAlgorithm.Encrypt( getValue(mongoRecord["ipAmendmentFees"]));
                    FinancialRequirement.PlainExtensionFee =  getValue(mongoRecord["ipExtensionFees"]);
                    FinancialRequirement.ExtensionFee = Command.CryptoAlgorithm.Encrypt( getValue(mongoRecord["ipExtensionFees"]));

                    
                    //ValidityTerm Requirements
                    ValidityRequirement.UomName = "Month";
                    ValidityRequirement.Quantity = Convert.ToInt32(getValue(mongoRecord["ipValidity"]));
                    ValidityRequirement.ExtensionAllowed = getLowerValue(mongoRecord["ipExtensionAllowed"]) == "yes" ? true : false;
                    ValidityRequirement.ExtensionPeriod = Convert.ToInt32(getValue(mongoRecord["ipExtensionPeriod"]));
                    ValidityRequirement.ExtensionPeriodUnitName = "Months";     // Hard coded till we have a separate column in sheet for this

                    //Quantity Allowed
                    if (RequestDTO.FactorCodeValuePair.Values.FirstOrDefault().FactorValue.ToString().Trim().ToLower() == Common.Constants.TradePurpose.ScreeningResearchTrial)
                    {
                        tarpRequirments.AllowedQuantity = getValue(mongoRecord["ipQuantityAllowed"]);
                    }
                }
                if (ipDocOptional != null && !ipDocOptional.Contains("NaN"))
                {
                    foreach (var lpco in ipDocOptional)
                    {
                        ipDocOptionalTrimmed.Add(lpco.Trim());
                    }

                    foreach (var doc in ipDocOptionalTrimmed)
                    {
                        var tempReq = new DocumentaryRequirement();

                        tempReq.Name = doc + " For Import Permit"; //replace DPP with collectionName 
                        tempReq.DocumentName = doc;
                        tempReq.IsMandatory = false;
                        tempReq.RequirementType = "Documentary";

                        tempReq.DocumentTypeCode = Command.SHRDUnitOfWork.DocumentTypeRepository.Where(new { Name = doc }).FirstOrDefault()?.Code;
                        tempReq.AttachedObjectFormatID = 1;

                        tarpDocumentRequirements.Add(tempReq);
                    }
                }

                if (ipDocRequirements != null && !ipDocRequirements.Contains("NaN"))
                {
                    foreach (var lpco in ipDocRequirements)
                    {
                        ipDocRequirementsTrimmed.Add(lpco.Trim());
                    }

                    foreach (var doc in ipDocRequirementsTrimmed)
                    {
                        var tempReq = new DocumentaryRequirement();

                        tempReq.Name = doc + " For Import Permit"; //replace DPP with collectionName 
                        tempReq.DocumentName = doc;
                        tempReq.IsMandatory = true;
                        tempReq.RequirementType = "Documentary";

                        tempReq.DocumentTypeCode = Command.SHRDUnitOfWork.DocumentTypeRepository.Where(new { Name = doc }).FirstOrDefault()?.Code;
                        tempReq.AttachedObjectFormatID = 1;

                        tarpDocumentRequirements.Add(tempReq);
                    }
                }
            }
            else if (documentClassification == DocumentClassificationCode.RELEASE_ORDER)
            {
                var roDocRequirements = new List<string>();
                var roDocRequirementsTrimmed = new List<string>();
                var roDocOptional = new List<string>();
                var roDocOptionalTrimmed = new List<string>();
                var ipReq = false;
                var psiReq = false;
                var psiRegReq = false;
                var psiReqMand = false;
                var psiRegReqMand = false;
                var psiRegScheme = string.Empty;
                var docClassificCode = string.Empty;

                if (Convert.ToInt32(RequestDTO.AgencyId) == (int)AgencyEnum.AQD)
                {
                    roDocRequirements = getListValue(mongoRecord["roMandatoryDocumentryRequirements"]);
                    roDocOptional = getListValue(mongoRecord["roOptionalDocumentryRequirements"]);
                    // ipReq = mongoRecord["ENLISTMENT OF SEED VARIETY REQUIRED (Yes/No)"].ToString().ToLower() == "yes";
                    //  docClassificCode = "PRD";

                    if (RequestDTO.IsFinancialRequirement)
                    {
                        AQDECFeeCalculateRequestDTO calculateECFeeRequest = new AQDECFeeCalculateRequestDTO();
                        calculateECFeeRequest.AgencyId = Convert.ToInt32(RequestDTO.AgencyId);
                        calculateECFeeRequest.HsCodeExt = RequestDTO.HsCode;
                        calculateECFeeRequest.Quantity = Convert.ToInt32(RequestDTO.Quantity);
                        calculateECFeeRequest.TradeTranTypeID = RequestDTO.TradeTranTypeID;
                        FactorData factorData = RequestDTO.FactorCodeValuePair["UNIT"];
                        if (factorData != null && !string.IsNullOrEmpty(factorData.FactorValueID))
                        {
                            calculateECFeeRequest.AgencyUOMId = Convert.ToInt32(factorData.FactorValueID);
                        }

                        AQDECFeeCalculation feeCalculation = new AQDECFeeCalculation(Command.UnitOfWork, Command.SHRDUnitOfWork, calculateECFeeRequest);
                        var responseModel = feeCalculation.CalculateECFee();
                        if (!responseModel.IsError)
                        {

                            FinancialRequirement.PlainAmount = responseModel.Model.Amount;
                            FinancialRequirement.Amount = Command.CryptoAlgorithm.Encrypt(FinancialRequirement.PlainAmount);
                            FinancialRequirement.PlainAmmendmentFee = responseModel.Model.Amount;
                            FinancialRequirement.AmmendmentFee = Command.CryptoAlgorithm.Encrypt(FinancialRequirement.PlainAmmendmentFee);
                        }
                        else
                        {
                            Log.Information("Response {@message}", responseModel.Error.InternalError.Message);
                            // return InternalServerErrorReply(responseModel.Error.InternalError.Message);
                        }

                    }
                }
                else if (Convert.ToInt32(RequestDTO.AgencyId) == (int)AgencyEnum.FSCRD)
                {
                    roDocRequirements = getListValue(mongoRecord["roMandatoryDocumentryRequirements"]);
                    roDocOptional = getListValue(mongoRecord["roOptionalDocumentryRequirements"]); 
                    ipReq = getLowerValue(mongoRecord["isProductRegistrationRequired"]) == "yes";
                    docClassificCode = "PRD";

                    //Financial Requirements
                    FinancialRequirement.PlainAmount = getValue(mongoRecord["roFees"]);
                    FinancialRequirement.Amount = Command.CryptoAlgorithm.Encrypt(getValue(mongoRecord["roFees"]));
                }
                 else if (Convert.ToInt32(RequestDTO.AgencyId) == (int)AgencyEnum.PSQ)
                {
                    roDocRequirements = getListValue(mongoRecord["roMandatoryDocumentryRequirements"]);
                    roDocOptional = getListValue(mongoRecord["roOptionalDocumentryRequirements"]);
                    ipReq = false;

                    //Financial Requirements
                    if (RequestDTO.IsFinancialRequirement)
                    {
                         var calculationBasis = Command.UnitOfWork.CalculationBasisRepository.Get().ToList();
                        var calculationSource = Command.UnitOfWork.CalculationSourceRepository.Get().ToList();
                        var feeConfigurationList = Command.UnitOfWork.LPCOFeeStructureRepository.GetFeeConfig(
                            lpcoRegulation.LpcoFeeStructureID
                        ).FirstOrDefault();

                        var feeConfig = new LPCOFeeCleanResp();
                        feeConfig.AdditionalAmount = feeConfigurationList.AdditionalAmount;
                        feeConfig.AdditionalAmountOn = calculationSource.Where(x=>x.ID ==feeConfigurationList.AdditionalAmountOn).Select(x=>x.Description).FirstOrDefault();//feeConfigurationList.AdditionalAmountOn;
                        feeConfig.Rate = feeConfigurationList.Rate;
                        feeConfig.CalculationBasis = calculationBasis.Where(x=>x.ID ==feeConfigurationList.CalculationBasis).Select(x=>x.Description).FirstOrDefault();
                        feeConfig.CalculationSource = calculationSource.Where(x=>x.ID ==feeConfigurationList.CalculationSource).Select(x=>x.Description).FirstOrDefault();//feeConfigurationList.CalculationSource;
                        feeConfig.MinAmount = feeConfigurationList.MinAmount;

                        var calculatedFee = new LPCOFeeCalculator(feeConfig, RequestDTO).Calculate();

                        FinancialRequirement.PlainAmount = calculatedFee.Fee.ToString();
                        FinancialRequirement.Amount = Command.CryptoAlgorithm.Encrypt(calculatedFee.Fee.ToString());
                        FinancialRequirement.AdditionalAmount = calculatedFee.AdditionalAmount;
                        FinancialRequirement.AdditionalAmountOn = calculatedFee.AdditionalAmountOn;
                    }
                }
                 else
                {
                    roDocRequirements = getListValue(mongoRecord["roMandatoryDocumentryRequirements"]);
                    roDocOptional = getListValue(mongoRecord["roOptionalDocumentryRequirements"]);
                    ipReq = getLowerValue(mongoRecord["ipRequired"]) == "yes";

                    // Check if HS Code is PSI related.  
                    var IsPSi = getLowerValue(mongoRecord["isPsi"]) == "yes";
                    if (IsPSi)
                    {
                        psiReq = getLowerValue(mongoRecord["psiRequired"]) == "yes";
                        psiRegReq = getLowerValue(mongoRecord["registationRequired"]) == "yes";
                        psiReqMand = getLowerValue(mongoRecord["psiRequiredMandatory"]) == "yes";
                        psiRegReqMand = getLowerValue(mongoRecord["registationRequiredMandatory"]) == "yes";
                        psiRegScheme = getLowerValue(mongoRecord["registationSchemeDescription"]);
                    }


                    //Financial Requirements
                    FinancialRequirement.PlainAmount = getValue(mongoRecord["roFees"]);
                    FinancialRequirement.Amount = Command.CryptoAlgorithm.Encrypt(getValue(mongoRecord["roFees"]));
                }
                if (roDocOptional != null && !roDocOptional.Contains("NaN"))
                {
                    foreach (var lpco in roDocOptional)
                    {
                        roDocOptionalTrimmed.Add(lpco.Trim());
                    }

                    foreach (var doc in roDocOptionalTrimmed)
                    {
                        var tempReq = new DocumentaryRequirement();

                        tempReq.Name = doc + " For " + "Release Order"; //replace DPP with collectionName 
                        tempReq.DocumentName = doc;
                        tempReq.IsMandatory = false;
                        tempReq.RequirementType = "Documentary";

                        tempReq.DocumentTypeCode = Command.SHRDUnitOfWork.DocumentTypeRepository.Where(new { Name = doc }).FirstOrDefault()?.Code;
                        tempReq.AttachedObjectFormatID = 1;

                        tarpDocumentRequirements.Add(tempReq);
                    }
                }

                if (roDocRequirements != null && !roDocRequirements.Contains("NaN"))
                {
                    foreach (var lpco in roDocRequirements)
                    {
                        var removespaces = lpco.Trim();
                        roDocRequirementsTrimmed.Add(removespaces.TrimEnd('\n'));
                    }

                    // roDocRequirementsTrimmed.Remove("Application on DPP prescribed form 20 [Rule 44(1) of PQR 2019]");
                    // roDocRequirementsTrimmed.Remove("Fee Challan");

                    //DocumentaryRequirements
                    foreach (var doc in roDocRequirementsTrimmed)
                    {
                        var tempReq = new DocumentaryRequirement();

                        tempReq.Name = doc + " For " + "Release Order"; //replace DPP with collectionName 
                        tempReq.DocumentName = doc;
                        tempReq.IsMandatory = true;
                        tempReq.RequirementType = "Documentary";

                        tempReq.DocumentTypeCode = Command.SHRDUnitOfWork.DocumentTypeRepository.Where(new { Name = doc }).FirstOrDefault()?.Code;
                        tempReq.AttachedObjectFormatID = 1;

                        tarpDocumentRequirements.Add(tempReq);
                    }
                }

                if (ipReq == true)
                {
                    var tempReq = new DocumentaryRequirement();
                    var ipDocRequired = Command.SHRDUnitOfWork.DocumentTypeRepository.Where(new { AgencyID = RequestDTO.AgencyId, documentClassificationCode = documentClassification, AttachedObjectFormatID = 2, AltCode = "C" }).FirstOrDefault();

                    tempReq.Name = ipDocRequired.Name + " For " + "Release Order"; //replace DPP with collectionName 
                    tempReq.DocumentName = ipDocRequired.Name;
                    tempReq.IsMandatory = true;
                    tempReq.RequirementType = "Documentary";
                    tempReq.DocumentTypeCode = ipDocRequired.Code;
                    tempReq.AttachedObjectFormatID = ipDocRequired.AttachedObjectFormatID;

                    tarpDocumentRequirements.Add(tempReq);

                }

                if (psiReq)
                {
                    var tempReq = new DocumentaryRequirement();

                    var psiDocRequired = Command.SHRDUnitOfWork.DocumentTypeRepository.Where(new
                    {
                        Code = "D58" // TODO : Remove hardcoded values
                    }).FirstOrDefault();

                    tempReq.Name = psiDocRequired.Name + " For " + "Release Order"; //replace DPP with collectionName 
                    tempReq.DocumentName = psiDocRequired.Name;
                    tempReq.IsMandatory = psiReqMand;
                    tempReq.RequirementType = "Documentary";
                    tempReq.DocumentTypeCode = psiDocRequired.Code;
                    tempReq.AttachedObjectFormatID = psiDocRequired.AttachedObjectFormatID;

                    tarpDocumentRequirements.Add(tempReq);

                }

                if (psiRegReq)
                {
                    var tempReq = new DocumentaryRequirement();
                    var documentCode = GetDocCodeByScheme(psiRegScheme);

                    var psiRegRequired = Command.SHRDUnitOfWork.DocumentTypeRepository.Where(new
                    {
                        Code = documentCode
                    }).FirstOrDefault();

                    tempReq.Name = psiRegRequired.Name + " For " + "Release Order"; //replace DPP with collectionName //
                    tempReq.DocumentName = psiRegRequired.Name;
                    tempReq.IsMandatory = psiRegReqMand;
                    tempReq.RequirementType = "Documentary";
                    tempReq.DocumentTypeCode = psiRegRequired.Code;
                    tempReq.AttachedObjectFormatID = psiRegRequired.AttachedObjectFormatID;

                    tarpDocumentRequirements.Add(tempReq);
                }
            }
            //for PythoCertificate = EC
            // NO EC in Agency 4 - FSCRD
            else if (documentClassification == DocumentClassificationCode.EXPORT_CERTIFICATE)
            {
                var ecDocRequirements = new List<string>();
                var ecDocRequirementsTrimmed = new List<string>();
                var ecDocOptional = new List<string>();
                var ecDocOptionalTrimmed = new List<string>();
                var premisesRegistrationRequired = false;
                var healthCertificateFeeRequired = false;
                var countries = new List<string>();

                ecDocRequirements = getListValue(mongoRecord["ecMandatoryDocumentryRequirements"]);
                ecDocOptional = getListValue(mongoRecord["ecOptionalDocumentryRequirements"]);
                
                
                if (Convert.ToInt32(RequestDTO.AgencyId) == (int)AgencyEnum.MFD)
                {
                    premisesRegistrationRequired = getLowerValue(mongoRecord["isPremiseRegistrationRequired"]) == "yes";
                }

                if (ecDocOptional != null && !ecDocOptional.Contains("NaN"))
                {
                    foreach (var lpco in ecDocOptional)
                    {
                        ecDocOptionalTrimmed.Add(lpco.Trim());
                    }

                    foreach (var doc in ecDocOptionalTrimmed)
                    {
                        var tempReq = new DocumentaryRequirement();

                        tempReq.Name = doc + " For Export Certificate";
                        tempReq.DocumentName = doc;
                        tempReq.IsMandatory = false;
                        tempReq.RequirementType = "Documentary";

                        tempReq.DocumentTypeCode = Command.SHRDUnitOfWork.DocumentTypeRepository.Where(new { Name = doc }).FirstOrDefault()?.Code;
                        tempReq.AttachedObjectFormatID = 1;
                        if (!string.IsNullOrEmpty(tempReq.DocumentTypeCode))
                        {
                            tarpDocumentRequirements.Add(tempReq);
                        }
                    }
                }

                if (ecDocRequirements != null && !ecDocRequirements.Contains("NaN"))
                {
                    foreach (var lpco in ecDocRequirements)
                    {
                        var removeSpaces = lpco.Trim();
                        ecDocRequirementsTrimmed.Add(removeSpaces.TrimEnd('\n'));
                    }

                    // roDocRequirementsTrimmed.Remove("Application on DPP prescribed form 20 [Rule 44(1) of PQR 2019]");
                    // roDocRequirementsTrimmed.Remove("Fee Challan");

                    //DocumentaryRequirements
                    foreach (var doc in ecDocRequirementsTrimmed)
                    {
                        var tempReq = new DocumentaryRequirement();

                        tempReq.Name = doc + " For Export Certificate";
                        tempReq.DocumentName = doc;
                        tempReq.IsMandatory = true;
                        tempReq.RequirementType = "Documentary";

                        tempReq.DocumentTypeCode = Command.SHRDUnitOfWork.DocumentTypeRepository.Where(new { Name = doc }).FirstOrDefault()?.Code;
                        tempReq.AttachedObjectFormatID = 1;
                        if (!string.IsNullOrEmpty(tempReq.DocumentTypeCode))
                        {
                            tarpDocumentRequirements.Add(tempReq);
                        }
                    }

                }
                if (premisesRegistrationRequired == true)
                {
                    // TODO : Attach this Later
                    var tempReq = new DocumentaryRequirement();
                    var premisesRegistration = Command.SHRDUnitOfWork.DocumentTypeRepository.Where(new
                    {
                        AgencyID = RequestDTO.AgencyId,
                        Code = "A09"
                    }).FirstOrDefault();

                    if (premisesRegistration != null)
                    {
                        tempReq.Name = premisesRegistration.Name + " For " + "Certificate";
                        tempReq.DocumentName = premisesRegistration.Name;
                        tempReq.IsMandatory = true; // Change this later 
                        tempReq.RequirementType = "Documentary";
                        tempReq.DocumentTypeCode = premisesRegistration.Code;
                        tempReq.AttachedObjectFormatID = premisesRegistration.AttachedObjectFormatID;
                        tarpDocumentRequirements.Add(tempReq);

                    }

                }
                if (RequestDTO.IsFinancialRequirement)
                {
                    //Financial Requirements
                    if (Convert.ToInt32(RequestDTO.AgencyId) == (int)AgencyEnum.DPP)
                    {
                        FinancialRequirement.PlainAmount = getValue(mongoRecord["ecFees"]);
                        FinancialRequirement.Amount = Command.CryptoAlgorithm.Encrypt(getValue(mongoRecord["ecFees"]));
                        FinancialRequirement.PlainAmmendmentFee = getValue(mongoRecord["ecAmendmentFees"]);
                        FinancialRequirement.AmmendmentFee = Command.CryptoAlgorithm.Encrypt(getValue(mongoRecord["ecAmendmentFees"]));
                    }
                    else if (Convert.ToInt32(RequestDTO.AgencyId) == (int)AgencyEnum.AQD)
                    {
                        AQDECFeeCalculateRequestDTO calculateECFeeRequest = new AQDECFeeCalculateRequestDTO();
                        calculateECFeeRequest.AgencyId = Convert.ToInt32(RequestDTO.AgencyId);
                        calculateECFeeRequest.HsCodeExt = RequestDTO.HsCode;
                        calculateECFeeRequest.Quantity = Convert.ToInt32(RequestDTO.Quantity);
                        calculateECFeeRequest.TradeTranTypeID = RequestDTO.TradeTranTypeID;
                        FactorData factorData = RequestDTO.FactorCodeValuePair["UNIT"];
                        if (factorData != null && !string.IsNullOrEmpty(factorData.FactorValueID))
                        {
                            calculateECFeeRequest.AgencyUOMId = Convert.ToInt32(factorData.FactorValueID);
                        }

                        AQDECFeeCalculation feeCalculation = new AQDECFeeCalculation(Command.UnitOfWork, Command.SHRDUnitOfWork, calculateECFeeRequest);
                        var responseModel = feeCalculation.CalculateECFee();
                        if (!responseModel.IsError)
                        {

                            FinancialRequirement.PlainAmount = responseModel.Model.Amount;
                            FinancialRequirement.Amount = Command.CryptoAlgorithm.Encrypt(FinancialRequirement.PlainAmount);
                            FinancialRequirement.PlainAmmendmentFee = responseModel.Model.Amount;
                            FinancialRequirement.AmmendmentFee = Command.CryptoAlgorithm.Encrypt(FinancialRequirement.PlainAmmendmentFee);
                        }
                        else
                        {
                            Log.Information("Response {@message}", responseModel.Error.InternalError.Message);
                            // return InternalServerErrorReply(responseModel.Error.InternalError.Message);
                        }
                    }
                    if (Convert.ToInt32(RequestDTO.AgencyId) == (int)AgencyEnum.MFD)
                    {
                        Log.Information("|{0}|{1}| RequestDTO.AgencyId  10 ", StrategyName, MethodID);


                        // TODO Fee releated stuff later
                        // // get fee  
                        // FinancialRequirement.PlainAmount = mongoRecord["Certificate of Quality and Origin Processing Fee (PKR)"].ToString();
                        // FinancialRequirement.Amount = Command.CryptoAlgorithm.Encrypt(mongoRecord["Certificate of Quality and Origin Processing Fee (PKR)"].ToString());
                        // FinancialRequirement.PlainAmount = mongoRecord["Health Certificate Fee(PKR)"].ToString();
                        // FinancialRequirement.Amount = Command.CryptoAlgorithm.Encrypt(mongoRecord["Health Certificate Fee(PKR)"].ToString());


                        string ECFeeString = mongoRecord["Certificate of Quality and Origin Processing Fee (PKR)"].ToString();

                        decimal ECFeeDecimal = 0.0m;
                        if (!string.IsNullOrEmpty(ECFeeString))
                            decimal.TryParse(ECFeeString, out ECFeeDecimal);


                        // The column that tells if Health Certificate is Fee Required (Conditional)
                        // Condition: If the destination country is from one of the countries in the following column, then fee is applied.
                        // "Names of Countries Requiring Health Certificate on prescribed format"
                        countries = mongoRecord["Codes of Countries Requiring Health Certificate on prescribed format"].ToString().Split('|').ToList();
                        Log.Information("|{0}|{1}| countries {@countries}", StrategyName, MethodID, countries);
                        if (countries.Contains(RequestDTO.DestinationCountryCode))
                        {
                            Log.Information("|{0}|{1}| contians {@RequestDTO.DestinationCountryCode}", StrategyName, MethodID, RequestDTO.DestinationCountryCode);
                            healthCertificateFeeRequired = true; // use later 

                            Log.Information("|{0}|{1}| ECFeeDecimal {@ECFeeDecimal}", StrategyName, MethodID, ECFeeDecimal);

                            string HealthCertFeeString = mongoRecord["Health Certificate Fee (PKR)"].ToString();
                            decimal HealthCertFeeDecimal = 0.0m;
                            if (!string.IsNullOrEmpty(HealthCertFeeString))
                                decimal.TryParse(HealthCertFeeString, out HealthCertFeeDecimal);

                            Log.Information("|{0}|{1}| ECFeeDecimal {@HealthCertFeeDecimal}", StrategyName, MethodID, HealthCertFeeDecimal);
                            ECFeeDecimal = HealthCertFeeDecimal + ECFeeDecimal;
                            Log.Information("|{0}|{1}| HealthCertFeeDecimal + ECFeeDecimal {@ECFeeDecimal}", StrategyName, MethodID, ECFeeDecimal);

                        }

                        FinancialRequirement.PlainAmount = ECFeeDecimal.ToString();
                        FinancialRequirement.Amount = Command.CryptoAlgorithm.Encrypt(ECFeeDecimal.ToString());

                    }
                }
            }
            tarpRequirments.DocumentaryRequirementList = tarpDocumentRequirements;
            tarpRequirments.FinancialRequirement = FinancialRequirement;
            tarpRequirments.ValidityRequirement = ValidityRequirement;

            response.Model = tarpRequirments;
            Log.Information("Tarp Requirments Response: {@response}", response);
            return response;
        }

        private string GetDocCodeByScheme(string PSIRegScheme)
        {
            var docCode = string.Empty;

            switch(PSIRegScheme)
            {
                case PSIRegisterationScheme.Form1:
                    return PSIRegisterationScheme.Form1_Certificate_DocType;
                case PSIRegisterationScheme.Form16:
                    return PSIRegisterationScheme.Form16_Certificate_DocType;
                case PSIRegisterationScheme.Form17:
                    return PSIRegisterationScheme.Form17_Certificate_DocType;
                default:
                    throw new ArgumentException($"PSI registeration scheme [\"{PSIRegScheme}\"] not recognized");
            }
        }
        private string getValue(JToken str){
            return str.Value<string>();
        }
        private string getLowerValue(JToken str){
            return str.Value<string>().ToLower();
        }
        private List<string> getListValue(JToken str){
            return str.ToObject<List<string>>();
        }
        public bool CheckIfLPCORequired(JObject mongoRecord, string requiredDocumentParentCode, out bool IsParenCodeValid)
        {
           
            switch (requiredDocumentParentCode)
            {
                case DocumentClassificationCode.IMPORT_PERMIT:
                    IsParenCodeValid = true;
                    return getLowerValue(mongoRecord["ipRequired"]) == "yes";

                case DocumentClassificationCode.RELEASE_ORDER:
                    IsParenCodeValid = true;
                    return getLowerValue(mongoRecord["roRequired"]) == "yes";

                case DocumentClassificationCode.EXPORT_CERTIFICATE:
                    IsParenCodeValid = true;
                   return getLowerValue(mongoRecord["ecRequired"]) == "yes";

                case DocumentClassificationCode.PRODUCT_REGISTRATION:
                IsParenCodeValid = true;
                return  getLowerValue(mongoRecord["isProductRegistrationRequired"]) == "yes";

                case DocumentClassificationCode.PREMISE_REGISTRATION:
                IsParenCodeValid = true;
                return  getLowerValue(mongoRecord["isPremiseRegistrationRequired"]) == "yes";

                default:
                    IsParenCodeValid = false;
                    return false;
            }
        }
        #endregion
    }
}
