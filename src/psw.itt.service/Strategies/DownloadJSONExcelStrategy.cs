using System.Collections.Generic;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;

namespace PSW.ITT.Service.Strategies
{
    public class DownloadJSONExcelStrategy : ApiStrategy<DownloadJSONExcelRequestDTO, DownloadJSONExcelResponseDTO>
    {
        #region Constructors
        public DownloadJSONExcelStrategy(CommandRequest commandRequest) : base(commandRequest)
        {

        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
                IDictionary<string, string> dictionary = new Dictionary<string, string>();


                var regulatoryData = Command.UnitOfWork.ProductRegulationRequirementRepository.GetRegulatoryDataByTradeTypeAndAgency(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID);
                // ResponseDTO = new List<GetRegulatoryDataResponseDTO>();

                // foreach (var x in regulatoryData)
                // {
                //     var responseData = new GetRegulatoryDataResponseDTO();
                //     var obj1 = JsonSerializer.Deserialize<dynamic>(x.RegulationJson);
                //     // var obj1 = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(x.RegulationJson);
                //     responseData.Data = obj1;
                //     responseData.ID = x.ID;
                //     ResponseDTO.Add(responseData);
                // }



                // ResponseDTO = ProductCodesWithOGAs.Select(item => Mapper.Map<GetProductCodeListWithAgenciesResponseDTO>(item)).ToList();



                // Prepare and return command reply
                return OKReply("Regulatory Data Fetched Successfully");
            }
            catch (ServiceException ex)
            {
                Log.Error("|{0}|{1}| Service exception caught: {2}", StrategyName, MethodID, ex.Message);
                return InternalServerErrorReply(ex);
            }
            catch (System.Exception ex)
            {
                Log.Error("|{0}|{1}| System exception caught: {2}", StrategyName, MethodID, ex.Message);
                return InternalServerErrorReply(ex);
            }
        }

        #endregion
    }
}