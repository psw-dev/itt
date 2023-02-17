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
using System.Text.Json;
using Newtonsoft.Json;
using PSW.ITT.Common.Constants;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace PSW.ITT.Service.Strategies
{
    public class GetAgencyListStrategy : ApiStrategy<GetListOfAgencyAgainstHsCodeRequest, GetListOfAgencyAgainstHsCodeResponse>
    {
        #region Constructors 
        public GetAgencyListStrategy(CommandRequest request) : base(request)
        {
            Reply = new CommandReply();
            this.Validator = new GetListOfAgencyAgainstHsCodeRequestDTOValidator();
        }
        #endregion 

        #region Distructors 
        ~GetAgencyListStrategy()
        {

        }
        #endregion 

        #region Strategy Excecution  

        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
        
                var tempAgencyList = Command.UnitOfWork.LPCORegulationRepository.Where(new {TradeTranTypeID = RequestDTO.tradeTranTypeId, HsCodeExt = RequestDTO.HsCode}).ToList();

                Log.Information("|{0}|{1}| Agency list fetched for database {@tempAgencyList}", StrategyName, MethodID, tempAgencyList);

                if (tempAgencyList == null || tempAgencyList.Count == 0)
                {
                    return BadRequestReply("Agency details not found against provided Hscode");
                }

                var agencyList = Command.SHRDUnitOfWork.AgencyRepository.Where(new{SoftDeleted=0 }).ToList();
                var agencyAccumelatedList = new List<AgencyList>();
                foreach(var agency in tempAgencyList)
                {
                    
                    JObject regulationJson = JObject.Parse(agency.RegulationJson);
                    var documentType = new DocumentType();
                    if(RequestDTO.tradeTranTypeId == (int)PSW.ITT.Common.Constants.TradeTranType.IMPORT){
                        if(regulationJson.ContainsKey("roRequired")){
                           documentType = Command.SHRDUnitOfWork.DocumentTypeRepository.Where(new{AgencyID=agency.AgencyID, DocumentClassificationCode=DocumentClassificationCode.RELEASE_ORDER, AltCode="C"}).FirstOrDefault();
                        }
                    }
                    else if(RequestDTO.tradeTranTypeId == (int)PSW.ITT.Common.Constants.TradeTranType.EXPORT){
                        if(regulationJson.ContainsKey("ecRequired")){
                            documentType = Command.SHRDUnitOfWork.DocumentTypeRepository.Where(new{AgencyID=agency.AgencyID, DocumentClassificationCode=DocumentClassificationCode.EXPORT_CERTIFICATE, AltCode="C"}).FirstOrDefault();
                        }
                    }
                   var productCodeAgencyLink = Command.UnitOfWork.ProductCodeAgencyLinkRepository.Where(new { ID = agency.ProductCodeAgencyLinkID}).FirstOrDefault();
                    
                   var productCode = Command.UnitOfWork.ProductCodeEntityRepository.Where(new { ID = productCodeAgencyLink.ProductCodeID}).FirstOrDefault();
                    // if (documentType != null)
                    // {
                    //     agency.RequiredDocumentCode = documentToInitiate.RequiredDocumentCode;      
                    // }
                    var agencyObject = new AgencyList{
                        Id = agency.AgencyID,
                        Name = agencyList.Where(x=>x.ID==agency.AgencyID).Select(a=>a.Name).FirstOrDefault(),
                        RequiredDocumentCode = documentType != null ? documentType.Code : null,
                        ItemDescription = productCode.Description
                    };  
                    agencyAccumelatedList.Add(agencyObject);
                }
                
                var distinctAgencyList = agencyAccumelatedList.Distinct(new objCompare()).ToList();
                ResponseDTO = new GetListOfAgencyAgainstHsCodeResponse
                {
                    AgencyList = distinctAgencyList
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
        
        public class objCompare : IEqualityComparer<AgencyList>
            {
                public bool Equals(AgencyList x, AgencyList y)
                {
                    return Equals(x.Id, y.Id);
                }

                public int GetHashCode(AgencyList obj)
                {
                    return obj.Id.GetHashCode();
                }
            }
    }
}
