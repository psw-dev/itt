using System.Linq;
using System.Collections.Generic;
using System.Data;
using System;
using System.Reflection;
using PSW.ITT.Service.DTO;
using PSW.Lib.Logs;
using OfficeOpenXml;
using PSW.itt.Common.Constants;
using PSW.itt.Common.Enums;
using System.Dynamic;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;
using psw.itt.service.Helpers;
using PSW.ITT.Service.Strategies;
using PSW.ITT.Service.Command;
using PSW.ITT.Common;
using PSW.ITT.Common.Enums;
using PSW.ITT.Data.Entities;
using PSW.ITT.Data.Sql.UnitOfWork;
using PSW.Common.Crypto;
using System.Security.Cryptography;
using System.Globalization;
using PSW.ITT.Common.Constants;
using PSW.ITT.Service.BusinessLogicLayer;
using PSW.ITT.Data.DTO;
using System.Text.Json;

namespace PSW.ITT.Service.Strategies
{
    public class UploadValidateProductCodeFileStrategy : ApiStrategy<UploadValidateProductCodeFileRequestDTO, UploadValidateProductCodeFileResponseDTO>
    {
        #region Constructors

        /// <summary>
        /// Strategy Constructor
        /// </summary>
        /// <param name="request">Takes CommandRequest</param>
        public UploadValidateProductCodeFileStrategy(CommandRequest request) : base(request)
        {
            Reply = new CommandReply();
            // Validator = new ProductCodeValidation();
            Log.Information("| Strategy Name : {StrategyName} | Method ID : {MethodID} | Constructor Called", StrategyName, MethodID);
        }

        #endregion

        #region Strategy Excecution

        /// <summary>
        /// Strategy Execute Method
        /// </summary>
        /// <returns>Returns CommandReply</returns>
        private int UserRoleId = 0;
        private int UserAgencyId = 0;

        public override CommandReply Execute()
        {
            try
            {
                var _culture = new CultureInfo("en-US");
                Log.Information("[{0}.{1}] Started", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                Log.Information("[{0}.{1}] Request DTO: {@RequestDTO}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, RequestDTO);

                var currentRole = Utility.GetCurrentUserRole(Command.UserClaims, Command.roleCode);

                if (currentRole == null || (currentRole.RoleCode != RoleCode.TRADER && currentRole.RoleCode != RoleCode.ITT_MANAGER))
                {
                    Log.Information("[{0}.{1}] Invalid Role", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return BadRequestReply("Invalid Role");
                }

                UserRoleId = currentRole.UserRoleID;

                // Check if previous upload is in progress

                

                DataTable dt = new DataTable();
                ResponseDTO = new UploadValidateProductCodeFileResponseDTO();

                var filePath = Utility.AESDecrypt256(RequestDTO.FilePath);
                dt = GetDataTableFromExcel(filePath);
                var outputTable = new DataTable();
                var formatTable = new DataTable();
                var errorColumnPosition = 0;
                var errorColumnIndexPosition = 0;
                var FileType=(int)FileTypeEnum.VALIDATE_PRODUCTCODE_REPORT;
                var FileTypeList = new List<int>();
                FileTypeList.Add(RequestDTO.FileType);
                var outputTableColumnNames = this.Command.UnitOfWork.SheetAttributeMappingRepository
                .Where(new { isActive = true ,SheetType = FileType})//.Where(x => x.SheetType == FileType)
                // .Select(x => new   { x.NameLong , x.Index})
                .OrderBy(x => x.Index).ToList();
            
                errorColumnPosition = dt.Rows[0].ItemArray.Length;
                errorColumnIndexPosition = dt.Rows[0].ItemArray.Length + 1;
                foreach (var cols in outputTableColumnNames)
                {
                    outputTable.Columns.Add(cols.NameLong.ToString(), typeof(string));
                }
                outputTable.Columns.Add("Error", typeof(string));
                dt.AcceptChanges();
                
                // if (dt.Rows.Count <= 1)
                // {
                //     Log.Information("[{0}.{1}] File not Uploaded Successfully as you are trying to upload an empty File", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                //     return BadRequestReply("File not Uploaded Successfully as you are trying to upload an empty File.");
                // }


                var propertyNameList = Command.UnitOfWork.SheetAttributeMappingRepository.Where(new { isActive = true ,SheetType = RequestDTO.FileType}).ToList();
                foreach (var cols in propertyNameList)
                {
                    formatTable.Columns.Add(cols.NameLong.ToString(), typeof(string));
                }
            if (RequestDTO.FileType == (short)FileTypeEnum.VALIDATE_PRODUCTCODE_TEMPLETE)
                {

                    Command.UnitOfWork.ProductCodeSheetUploadHistoryRepository.SetIsCurrent(FileTypeList);
                    var fileCheck = IsFileColumnsCorrect((dt.Rows[0].ItemArray).Select(x => x.ToString()).ToList(), propertyNameList.Select(x=>x.NameLong).ToList());
                    short validationStatus = (short)ProductCodeSheetUploadStatusEnum.VALIDATED;
                    string processingResponse = "Successfully Validated.";
                    if (fileCheck.Count > 0)
                    {
                        validationStatus = (short)ProductCodeSheetUploadStatusEnum.STRUCTURE_VALIDATION_FAILED;

                        processingResponse = fileCheck.Count == 1 ? string.Concat("Error in File, ", string.Join(",", fileCheck.Select(kv => kv.Key).ToArray())) : string.Concat("Error in File, ", string.Join(",", fileCheck.Select(kv => kv.Key + " column name should be " + kv.Value).ToArray()));

                        formatTable.Columns.Add("Error", typeof(string));
                        formatTable.Columns.Add(processingResponse, typeof(string));
                        DataRow row = formatTable.NewRow();
                        row.ItemArray = dt.Rows[0].ItemArray;
                        row[errorColumnPosition] = "Error";
                        row[errorColumnIndexPosition] = processingResponse;

                        formatTable.Rows.Add(row);
                        // return BadRequestReply($"Error in File.");
                    }
                    var validationFileUploadHistory = new ProductCodeSheetUploadHistory
                    {
                        AttachedFileID = RequestDTO.FileID,
                        Name = RequestDTO.FileName,
                        TotalRecordsCount = dt.Rows.Count - 1,
                        AgencyID = (short)RequestDTO.AgencyId,
                        SheetType = RequestDTO.FileType,
                        // TradeTranTypeID = RequestDTO.TradeTranTypeID,
                        DuplicateRecordsCount = 0,
                        IsCurrent = true,
                        DisputedRecordsCount = 0,
                        ProcessedRecordsCount = 0,
                        ProductCodeSheetUploadStatusID = validationStatus,
                        ProcessingResponse = processingResponse,
                        CreatedBy = UserRoleId,
                        UpdatedBy = UserRoleId,
                        CreatedOn = DateTime.Now,
                        UpdatedOn = DateTime.Now
                    };

                    long validationFileUploadHistoryID = Command.UnitOfWork.ProductCodeSheetUploadHistoryRepository.Add(validationFileUploadHistory);


                    if (fileCheck.Count > 0)
                    {
                        ResponseDTO.GridColumns = GetGridFormatColumns(dt.Rows[0].ItemArray.ToList(), processingResponse, outputTableColumnNames.Select(x=>x.NameLong).ToList());
                        ResponseDTO.Data = GetRegisteredRecords( new DataTable(), outputTableColumnNames);
                        ResponseDTO.DisputedRecordCount = 0;
                        ResponseDTO.StatusID = validationStatus;
                        ResponseDTO.DuplicateRecordCount = 0;
                        ResponseDTO.TotalRecordCount = dt.Rows.Count;
                        ResponseDTO.ProcessedRecordsCount = 0;
                        Log.Information($"| Strategy Name : {StrategyName} | Method ID : {MethodID} | File Structure Validation Failed.");
                        return BadRequestReply("Validation Failed.");

                    }

                    dt.Rows.Remove(dt.Rows[0]);

                    //     //TODO will uncomment
                    var columnsCheck = CheckIsMandatoryColumnsAvailable(dt, propertyNameList);

                    if (columnsCheck != "")
                    {
                        Log.Information("[{0}.{1}] Error in File, Column {2} could not be null.", this.GetType().Name, MethodBase.GetCurrentMethod().Name, columnsCheck);
                        return BadRequestReply($"Error in File, Column {columnsCheck} could not be null.");

                    }






                    //    var duplicateTable = GetDuplicateRecords(dt, duplicateTable, errorColumnPosition);
                    var validationList = Command.UnitOfWork.AttributeValidationMappingRepository.GetAssociatedValidationList(propertyNameList.Select(x => x.ID).ToList()).ToList();
                    int rowIndex = 0;

                    foreach (DataRow d in dt.Rows)
                    {
                        string error = "";
                        string productCode = d["Product Code"].ToString();
                        string hsCode = d["HS Code"].ToString();
                       
                        DataRow row = outputTable.NewRow();
                        // if (error == "")
                        // {


                            for (var i = 0; i < d.ItemArray.Count(); i++)
                            {
                                List<Data.DTO.ProductCodeValidationList> validation = validationList.Where(x => x.Index == i ).ToList();
                                if (validation.Count > 0)
                                {
                                    ProductCodeFileValidation PCValidator = new ProductCodeFileValidation(hsCode,d.ItemArray[i].ToString(), dt.Columns[i].ToString(), validation, Command, RequestDTO.AgencyId, 0);
                                    var validated = PCValidator.validate();

                                    if (validated != "")
                                    {
                                        error = error == "" ? validated : String.Concat(error, ", ", validated);
                                        // row.ItemArray[0].GetType = d.ItemArray;
                                        // row[errorColumnPosition] = validated;
                                        // row[errorColumnIndexPosition] = rowIndex + 1;
                                        // dispuedTable.Rows.Add(row);
                                    }
                                    
                                }
                            }
                        // }

                        // var duplicateCheckIndexList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType == (short)FileTypeEnum.UPDATE_REGULATIONS_TEMPLATE ? (short)FileTypeEnum.ADD_REGULATIONS_TEMPLATE : RequestDTO.FileType).Where(x => x.CheckDuplicate == true).Select(x => x.Index).ToList();

                        // foreach (DataRow drow in outputTable.Rows)
                        // {
                        //     var count = 0;
                        //     foreach (int a in duplicateCheckIndexList)
                        //     {
                        //         if (drow[a - 1].ToString() == d[a - 1].ToString()) count++;
                        //     }
                        //     if (count == duplicateCheckIndexList.Count)
                        //     {
                        //         error = error == "" ? drow[errorColumnPosition].ToString() : String.Concat(error, ", ", drow[errorColumnPosition]);
                        //     }
                        // }
                        string rowStatus="";
                        string tradeTranType="";
                        string availability="No";
                        // if (error != "")
                        // {

                            // row.ItemArray = d.ItemArray;
                            // row[errorColumnPosition] = error;
                            // row[errorColumnIndexPosition] = rowIndex + 1;
                            // outputTable.Rows.Add(row);
                        // }
                        
                          if (error == "")
                        {
                            var record = Command.UnitOfWork.ProductCodeEntityRepository.Where(new{HsCode=hsCode,HSCodeExt=productCode}).OrderByDescending(x=>x.ID).ToList();
                            availability = "Yes";
                            // tradeTranType = value.TradeTranTypeID == 1 ? "Import": value.TradeTranTypeID== 2 ? "Export" : value.TradeTranTypeID == 3 ? "Trasit" : value.TradeTranTypeID == 4 ? "Both" : null;
                            foreach(var item in record){
                                if (item.EffectiveFromDt<=DateTime.Now && item.EffectiveThruDt>=DateTime.Now ){
                                    rowStatus = "Active";
                                    }
                                else{
                                    rowStatus = rowStatus ==""? "In Active":rowStatus;
                                }
                                var tradeType=item.TradeTranTypeID == 1 ? "Import": item.TradeTranTypeID == 2 ? "Export" : item.TradeTranTypeID == 3 ? "Trasit" : "Both";
                                tradeTranType = tradeTranType.Contains(tradeType) ? tradeTranType : String.Join(", " ,tradeType ) ;
                                
                                
                            }
                            
                        }
                    // }
            
                    row[0]= hsCode;
                    row[1]= productCode;
                    row[2]= tradeTranType;
                    row[3]= availability;
                    row[4]= rowStatus;
                    outputTable.Rows.Add(row);
                
                    }


                //     var duplicateRecordCount = dt.Rows.Count - duplicateTable.Rows.Count;
                //     // DataTable mergeDuplicateAndDisputedTable = new DataTable();
                //     // mergeDuplicateAndDisputedTable.Merge(dispuedTable);
                //     // mergeDuplicateAndDisputedTable.Merge(duplicateTable);
                    short status = (short)ProductCodeSheetUploadStatusEnum.DATA_VALIDATED;
                //     // ValidateNationalRegistration validateRegistrationData = new ValidateNationalRegistration();
                    processingResponse = "Data Successfully Validated.";
                //     // Update National RegisterFileHistory
                    var fileUploadHistory = new ProductCodeSheetUploadHistory();
                    if(dt.Rows.Count==0){
                        processingResponse = "No Record Found in the Sheet.";
                        status = (short)ProductCodeSheetUploadStatusEnum.DATA_VALIDATION_FAILED;
                    }
                    // else if (dt.Rows.Count>0 &&(duplicateTable.Rows.Count > 0 || dispuedTable.Rows.Count > 0))
                    // {
                    //     processingResponse = "Data Validation Failed.";
                    //     status = (short)ProductCodeSheetUploadStatusEnum.DATA_VALIDATION_FAILED;
                    // }
                    fileUploadHistory.AttachedFileID = RequestDTO.FileID;
                    fileUploadHistory.Name = RequestDTO.FileName;
                    fileUploadHistory.AgencyID = (short)RequestDTO.AgencyId;
                    fileUploadHistory.TotalRecordsCount = dt.Rows.Count;
                    fileUploadHistory.ProcessedRecordsCount = dt.Rows.Count;
                    fileUploadHistory.DuplicateRecordsCount = 0;
                    fileUploadHistory.ProcessingResponse = processingResponse;
                    fileUploadHistory.TradeTranTypeID = 0;
                    fileUploadHistory.IsCurrent = true;
                    fileUploadHistory.DisputedRecordsCount = 0;
                    fileUploadHistory.ProductCodeSheetUploadStatusID = status;
                    fileUploadHistory.CreatedBy = UserRoleId;
                    fileUploadHistory.SheetType = RequestDTO.FileType;
                    fileUploadHistory.UpdatedBy = UserRoleId;
                    fileUploadHistory.CreatedOn = DateTime.Now;
                    fileUploadHistory.UpdatedOn = DateTime.Now;

                    long fileUploadHistoryID = Command.UnitOfWork.ProductCodeSheetUploadHistoryRepository.Add(fileUploadHistory);
                    var fileUploadHistoryRecord = Command.UnitOfWork.ProductCodeSheetUploadHistoryRepository.Where(new { ID = fileUploadHistoryID }).FirstOrDefault();
                //     // ResponseDTO = new UploadConfigrationFileResponseDTO();
                    if (fileUploadHistoryID < 1)
                    {
                        Log.Information("[{0}.{1}] Error in File uploading.", this.GetType().Name, MethodBase.GetCurrentMethod().Name); //columnsCheck
                        return BadRequestReply($"Error in File uploading, Please try again later.");
                    }
                        ResponseDTO.StatusID = status;
                    if (outputTable.Rows.Count >= 0 && dt.Rows.Count>0)
                    {


                        ResponseDTO.DisputedRecordCount = 0;
                        ResponseDTO.DuplicateRecordCount = 0;
                        ResponseDTO.TotalRecordCount = dt.Rows.Count;
                        ResponseDTO.GridColumns = GetGridColumns(outputTableColumnNames);
                        ResponseDTO.Data = GetRegisteredRecords(outputTable,  outputTableColumnNames);
                        ResponseDTO.ProcessedRecordsCount = fileUploadHistoryRecord.ProcessedRecordsCount == null ? 0 : fileUploadHistoryRecord.ProcessedRecordsCount;
                        Log.Information($"| Strategy Name : {StrategyName} | Method ID : {MethodID} | Upload Successfully");
                        return OKReply("Validated Successfully.");

                    }
                

                }

                return OKReply("Upload Successfully.");
            }
            catch (System.Exception ex)
            {
                Log.Error("[{0}.{1}] {2}-{3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex, ex.StackTrace);
                Command.UnitOfWork.Rollback();
                return InternalServerErrorReply(ex);
            }
            finally
            {
                Command.UnitOfWork.CloseConnection();
            }
        }

        #endregion
        #region Methods  
        private List<dynamic> GetRegisteredRecords(DataTable dt, List<SheetAttributeMapping > outputTableColumnNames)
        {
            try
            {
                List<dynamic> gridData = new List<dynamic>();

// 
                // var propertyNameList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType == (short)FileTypeEnum.UPDATE_REGULATIONS_TEMPLATE ? (short)FileTypeEnum.ADD_REGULATIONS_TEMPLATE : RequestDTO.FileType).OrderBy(x => x.Index).ToList();


                int rowIndex = 0;
                foreach (DataRow drow in dt.Rows)
                {
                    rowIndex += 1;
                    IDictionary<string, object> expandoDict = new ExpandoObject();
                    foreach (var x in outputTableColumnNames)
                    {
                        expandoDict.Add(x.NameShort, drow[x.Index ]);

                    }
                    if (RequestDTO.ActionID == (short)ActionID.VALIDATE)
                    {
                        expandoDict.Add("error", drow[outputTableColumnNames.Count]);
                        // expandoDict.Add("rowIndex", rowIndex + 1);
                    }
                    gridData.Add(expandoDict);

                }
                return gridData;


            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private List<GridColumns> GetGridFormatColumns(List<object> columnNames, string processingResponse, List<string> outputTableColumnNames)
        {
            try
            {
                List<GridColumns> gridColumns = new List<GridColumns>();

                // var propertyNameList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType == (short)FileTypeEnum.UPDATE_REGULATIONS_TEMPLATE ? (short)FileTypeEnum.ADD_REGULATIONS_TEMPLATE : RequestDTO.FileType).OrderBy(x => x.Index).ToList();
                foreach (var x in outputTableColumnNames)
                {
                    var column = new GridColumns();
                    column.Field = x.ToString();
                    column.Title = x.ToString();
                    column.Editor = "string";
                    column.Width = "90px";
                    gridColumns.Add(column);
                }

                var columnError = new GridColumns
                {
                    Field = "error",
                    Title = processingResponse,
                    Editor = "string",
                    Width = "400px"
                };
                gridColumns.Add(columnError);


                return gridColumns;

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        private List<GridColumns> GetGridColumns(List<SheetAttributeMapping > propertyNameList)
        {
            try
            {
                List<GridColumns> gridColumns = new List<GridColumns>();

                // var propertyNameList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType == (short)FileTypeEnum.UPDATE_REGULATIONS_TEMPLATE ? (short)FileTypeEnum.ADD_REGULATIONS_TEMPLATE : RequestDTO.FileType).OrderBy(x => x.Index).ToList();
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
     
        private IDictionary<string, string> IsFileColumnsCorrect(List<string> headerRow,List<string> propertyNameList)
        {
            var res = new Dictionary<string, string>();
            var arraysAreEqual = Enumerable.SequenceEqual(propertyNameList, headerRow);
            if (propertyNameList.Count != headerRow.Count)
            {
                res.Add("column count mismatch", "");
                return res;
            }
            for (int i = 0; i < propertyNameList.Count; i++)
            {
                if (propertyNameList[i] != headerRow[i].Trim())
                {
                    res.Add(headerRow[i], propertyNameList[i]);
                }
            }
            return res;

        }

        private string CheckIsMandatoryColumnsAvailable(DataTable dt,List<SheetAttributeMapping > propertyNameList)
        {
            // var dbColumns = this.Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType == (short)FileTypeEnum.UPDATE_REGULATIONS_TEMPLATE ? (short)FileTypeEnum.ADD_REGULATIONS_TEMPLATE : RequestDTO.FileType).Where(x => x.IsMandatory = true).ToList();


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                foreach (var allItems in propertyNameList)
                {

                    if (row[allItems.Index ].ToString() == "")
                    { return $"'{allItems.NameLong}' at Row Number {i + 1}"; }
                }
            }
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="hasHeader"></param>
        /// <returns></returns>
        public DataTable GetDataTableFromExcel(string path, bool hasHeader = true)
        {
            try
            {
                Log.Information($" Start reading excel file");

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


                using (var pck = new OfficeOpenXml.ExcelPackage())
                {
                    //using (var stream = this.Command.file)
                    { pck.Load(this.Command.file.OpenReadStream()); }
                    var ws = pck.Workbook.Worksheets.First();


                    DataTable tbl = new DataTable();
                    foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                    {
                        tbl.Columns.Add(hasHeader ? firstRowCell.Text : string.Format("Column {​​​​​​​0}​​​​​​​", firstRowCell.Start.Column));
                    }
                    var startRow = 1;
                    for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        DataRow row = tbl.Rows.Add();
                        foreach (var cell in wsRow)
                        {
                            //Have to Remove
                            if (cell.Start.Column > ws.Dimension.End.Column)
                            { break; }

                            row[cell.Start.Column - 1] = cell.Text;
                        }
                    }
                    Log.Information($" Excel file generated.");
                    return tbl;
                }

            }
            catch (SystemException ex)
            {
                throw ex;
            }
        }


        #endregion

    }
}