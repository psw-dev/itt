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
    public class GetPCTCodeListStrategy : ApiStrategy<GetPCTCodeListRequest, GetPCTCodeListResponse>
    {
        #region Constructors 
        public GetPCTCodeListStrategy(CommandRequest request) : base(request)
        {
            Reply = new CommandReply();
            this.Validator = new GetPCTCodeListRequestDTOValidator();
        }
        #endregion 

        #region Distructors 
        ~GetPCTCodeListStrategy()
        {

        }
        #endregion 

        #region Strategy Excecution  

        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
        
                var tempPctCodeList = Command.UnitOfWork.RegulatedHSCodeRepository.GetPCTCodeList(RequestDTO.HsCode, RequestDTO.TradeTranTypeID);

                if (tempPctCodeList == null || tempPctCodeList.Count == 0)
                {
                    ResponseDTO = new GetPCTCodeListResponse
                    {
                        Message = "Product codes does not exist for the provided hscode."
                    };

                    Log.Information("|{0}|{1}| Response DTO : {@ResponseDTO}", StrategyName, MethodID, ResponseDTO);

                    return OKReply();
                }

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

    }
}
