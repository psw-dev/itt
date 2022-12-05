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
                // IDictionary<string, string> dictionary = new Dictionary<string, string>();

                ResponseDTO = new DownloadJSONExcelResponseDTO();
                List<dynamic> Data = new List<dynamic>();
                var regulatoryData = Command.UnitOfWork.ProductRegulationRequirementRepository.GetRegulatoryDataByTradeTypeAndAgency(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID);
                // ResponseDTO = new List<GetRegulatoryDataResponseDTO>();

                foreach (var x in regulatoryData)
                {
                    var obj1 = JsonSerializer.Deserialize<dynamic>(x.RegulationJson);
                    // var obj1 = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(x.RegulationJson);
                    // responseData.Data = obj1;
                    Data.Add(obj1);
                }
                ResponseDTO.Data = Data;


                var dispuedTable = new DataTable();

                ResponseDTO.GridColumns = GetGridColumns(1);


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

        private List<GridColumns> GetGridColumns(short actionID)
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


                var propertyNameList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, 1).OrderBy(x => x.Index).ToList();


                int rowIndex = 0;
                foreach (DataRow drow in dt.Rows)
                {
                    rowIndex += 1;
                    IDictionary<string, object> expandoDict = new ExpandoObject();
                    foreach (var x in propertyNameList)
                    {
                        expandoDict.Add(x.NameShort, drow[x.Index - 1]);

                    }
                    // if (RequestDTO.ActionID == (short)ActionID.VALIDATE)
                    // {
                    //     expandoDict.Add("error", drow[propertyNameList.Count]);
                    //     expandoDict.Add("rowIndex", rowIndex + 1);
                    // }
                    gridData.Add(expandoDict);

                }
                return gridData;


            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

    }
}