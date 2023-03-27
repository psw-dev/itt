using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text.Json;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;   
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

                ResponseDTO = new DownloadJSONExcelResponseDTO();
                List<dynamic> Data = new List<dynamic>();
                var regulatoryData = Command.UnitOfWork.ProductRegulationRequirementRepository.GetRegulatoryDataByTradeTypeAndAgency(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID);

                foreach (var x in regulatoryData)
                {
                    var json = JObject.Parse(x.RegulationJson);
                    foreach (var item in json) 
                    {
                        if(item.Value.Count()>0)
                        {   var a = "";
                            var jobj = (JArray)item.Value;
                            foreach(JToken  jprop in jobj) {
                                if(item.Key.Contains("Fees")){
                                    a = a=="" ? String.Join( "|",jprop.ToString()) : String.Join( "|",a,jprop.ToString());
                                }
                                else{
                                    a = a=="" ? String.Join( ",",jprop.ToString()) : String.Join( ",",a,jprop.ToString());
                                }
                            }
                        json[item.Key] = (JToken)a;
                        }
                    }
                    var obj1 = System.Text.Json.JsonSerializer.Deserialize<dynamic>(json.ToString());
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

    }
}