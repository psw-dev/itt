using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;

namespace PSW.ITT.Service.Strategies
{
    public class GetRegulatoryDataStrategy : ApiStrategy<GetRegulatoryDataRequestDTO, List<GetRegulatoryDataResponseDTO>>
    {
        #region Constructors
        public GetRegulatoryDataStrategy(CommandRequest commandRequest) : base(commandRequest)
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
                ResponseDTO = new List<GetRegulatoryDataResponseDTO>();

                foreach (var x in regulatoryData)
                {
                    var responseData = new GetRegulatoryDataResponseDTO();
                    var obj1 = JsonSerializer.Deserialize<dynamic>(x.RegulationJson);
                    // var obj1 = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(x.RegulationJson);
                    responseData.Data = obj1;
                    responseData.ID = x.ID;
                    ResponseDTO.Add(responseData);
                }



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

        private List<GridColumns> GetGridColumns()
        {
            try
            {
                List<GridColumns> gridColumns = new List<GridColumns>();

                var propertyNameList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, 1).OrderBy(x => x.Index).ToList();
                foreach (var x in propertyNameList)
                {
                    var column = new GridColumns();
                    column.Field = x.NameShort;
                    column.Title = x.NameLong;
                    column.Editor = "string";
                    column.Width = "90px";
                    gridColumns.Add(column);
                }


                return gridColumns;

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private List<dynamic> GetRegisteredRecords(DataTable dt)
        {
            try
            {
                List<dynamic> gridData = new List<dynamic>();


                var propertyNameList = Command.UnitOfWork.SheetAttributeMappingRepository.Where(new { isActive = 1 }).OrderBy(x => x.Index).ToList();


                int rowIndex = 0;
                foreach (DataRow drow in dt.Rows)
                {
                    rowIndex += 1;
                    IDictionary<string, object> expandoDict = new ExpandoObject();
                    foreach (var x in propertyNameList)
                    {
                        expandoDict.Add(x.NameShort, drow[x.Index]);

                    }
                    expandoDict.Add("error", drow[propertyNameList.Count]);
                    expandoDict.Add("rowIndex", rowIndex + 1);
                    gridData.Add(expandoDict);

                }
                return gridData;


            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}