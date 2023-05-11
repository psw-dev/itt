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
    public class GetFactorLOVItemsStrategy : ApiStrategy<GetFactorLovItemsRequest, GetFactorLovItemsResponse>
    {
        #region Constructors 
        public GetFactorLOVItemsStrategy(CommandRequest request) : base(request)
        {
            Reply = new CommandReply();
            this.Validator = new GetFactorLOVItemsRequestDTOValidator();
        }
        #endregion 

        #region Distructors 
        ~GetFactorLOVItemsStrategy()
        {

        }
        #endregion 

        #region Strategy Excecution  

        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
                

                var getFactor = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping((short)RequestDTO.TradeTranTypeID, (short)RequestDTO.AgencyId, 1).Where(x => x.CheckDuplicate == true).ToList();
                getFactor.RemoveAll(x => x.NameShort.Contains("hsCode"));
                getFactor.RemoveAll(x => x.NameShort.Contains("productCode"));
                
                var tempFactorDatalist = new List<FactorLOVItemsData>();
                    foreach(var i in RequestDTO.FactorList){
                        var factorLOVItemsList = new FactorLOVItemsData();
                        var item = Command.UnitOfWork.LPCORegulationRepository.GetFactorLovItemList(RequestDTO.AgencyId,RequestDTO.TradeTranTypeID,RequestDTO.HSCodeExt);
                        factorLOVItemsList.FactorLOVItems=item;
                        factorLOVItemsList.FactorLabel=getFactor.FirstOrDefault().NameLong;
                        factorLOVItemsList.FactorID= i.FactorId;
                        tempFactorDatalist.Add(factorLOVItemsList);
                    }
           
                

                if (tempFactorDatalist == null || tempFactorDatalist.Count == 0)
                {
                    ResponseDTO = new GetFactorLovItemsResponse
                    {
                        FactorLOVItemsList = new List<FactorLOVItemsData>()
                    };
                    return OKReply("LOV data not available for provided factor list");
                }

                ResponseDTO = new GetFactorLovItemsResponse
                {
                    FactorLOVItemsList = tempFactorDatalist
                };

                Log.Information("|{0}|{1}| Respose DTO {@ResponseDTO}", StrategyName, MethodID, ResponseDTO);
                // Log.Information("[{0}.{1}] Respose: {@ResponseDTO}", GetType().Name, MethodBase.GetCurrentMethod().Name, ResponseDTO);
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
