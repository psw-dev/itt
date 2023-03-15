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
    public class GetRegulatedHSCodeExtListStrategy : ApiStrategy<GetRegulatedHscodeListRequest, GetRegulatedHSCodeExtListResponse>
    {
        #region Constructors 
        private int agencyId = 0;
        public GetRegulatedHSCodeExtListStrategy(CommandRequest request) : base(request)
        {
            Reply = new CommandReply();
            this.Validator = new GetRegulatedHscodeListRequestDTOValidator();
            var claims = request.UserClaims.Where(x => x.Type == "agencyId").FirstOrDefault();
            if (claims != null)
            {
                agencyId = Convert.ToInt32(claims.Value);
            }
        }
        #endregion 

        #region Distructors 
        ~GetRegulatedHSCodeExtListStrategy()
        {

        }
        #endregion 

        #region Strategy Excecution  

        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);

                var hsCodeList = new List<ViewRegulatedHsCodeExt>();
        
                //Get Regulated Hscode list filtered on base of AgencyId and chapter
                if (agencyId != 0 && RequestDTO.Chapter != null && RequestDTO.TradeTranTypeID == null)
                {
                    hsCodeList = Command.UnitOfWork.ProductCodeAgencyLinkRepository.GetHsCodeExtList(agencyId, RequestDTO.Chapter);

                    if (hsCodeList == null || hsCodeList.Count == 0)
                    {
                        return BadRequestReply("Hscodes not available against provided Agency and chapter");
                    }
                }
                
                //Get Regulated Hscode list filtered on base of AgencyId 
                if (agencyId != 0 && RequestDTO.TradeTranTypeID == null)
                {
                    hsCodeList = Command.UnitOfWork.ProductCodeAgencyLinkRepository.GetHsCodeExtList(agencyId);

                    if (hsCodeList == null || hsCodeList.Count == 0)
                    {
                        return BadRequestReply("Hscodes not available against provided Agency");
                    }
                }

                else
                {
                    hsCodeList = Command.UnitOfWork.ProductCodeAgencyLinkRepository.GetHsCodeExtList();

                    if (hsCodeList == null || hsCodeList.Count == 0)
                    {
                        return BadRequestReply("Hscodes not available against provided Agency");
                    }
                }

                ResponseDTO = new GetRegulatedHSCodeExtListResponse
                {
                    RegulatedHsCodeExtList = hsCodeList
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

    }
}
