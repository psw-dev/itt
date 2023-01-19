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
                        row[dt.Columns.Count] = "Error";
                        row[dt.Columns.Count + 1] = processingResponse;

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
                        // string effectiveFromDt = d["Effective From Date"].ToString();
                        // string effectiveThruDt = d["Effective Thru Date"].ToString();

                        // var duplicateCheckList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType == (short)FileTypeEnum.UPDATE_REGULATIONS_TEMPLATE ? (short)FileTypeEnum.ADD_REGULATIONS_TEMPLATE : RequestDTO.FileType).Where(x => x.CheckDuplicate == true).ToList();
                        // duplicateCheckList.RemoveAll(x => x.NameLong.Contains("HSCode"));
                        // duplicateCheckList.RemoveAll(x => x.NameLong.Contains("Product Code"));
                        // if (RequestDTO.FileType == (short)FileTypeEnum.ADD_REGULATIONS_TEMPLATE)
                        // {
                        //     var recordAlreadyExistInTheSystem = Command.UnitOfWork.LPCORegulationRepository.CheckIfRecordAlreadyExistInTheSystem(hsCode, productCode, RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, d[duplicateCheckList.FirstOrDefault().NameLong].ToString()); //, Convert.ToDateTime(effectiveFromDt), Convert.ToDateTime(effectiveThruDt)
                        //     if (recordAlreadyExistInTheSystem != null)
                        //     {
                        //         error = "Record already exist in the system.";
                        //     }
                        // }
                        // else if (RequestDTO.FileType == (short)FileTypeEnum.UPDATE_REGULATIONS_TEMPLATE || RequestDTO.FileType == (short)FileTypeEnum.INACTIVATE_REGULATIONS_TEMPLATE)
                        // {

                        //     var recordAlreadyExistInTheSystem = Command.UnitOfWork.LPCORegulationRepository.CheckIfRecordAlreadyExistInTheSystem(hsCode, productCode, RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, d[duplicateCheckList.FirstOrDefault().NameLong].ToString());
                        //     if (recordAlreadyExistInTheSystem == null)
                        //     {
                        //         error = "Record does not exist in the system.";
                        //     }
                        // }
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
                

                //     ResponseDTO.GridColumns = GetGridColumns(RequestDTO.ActionID);
                //     ResponseDTO.Data = GetRegisteredRecords(dispuedTable);



                //     ResponseDTO.DisputedRecordCount = dispuedTable.Rows.Count;
                //     ResponseDTO.DuplicateRecordCount = duplicateTable.Rows.Count;
                //     ResponseDTO.TotalRecordCount = dt.Rows.Count;
                //     ResponseDTO.ProcessedRecordsCount = fileUploadHistoryRecord.ProcessedRecordsCount == null ? 0 : fileUploadHistoryRecord.ProcessedRecordsCount; ;

                //     if (dispuedTable.Rows.Count > 0 || dt.Rows.Count==0)
                //     {
                //         return BadRequestReply("Validation Failed.");
                //     }
                //     else
                //     {

                        // return OKReply("Validated Successfully.");
                //     }

                //     // return OKReply("Upload Successfully.");
                // }
                // else if (RequestDTO.ActionID == (short)ActionID.SUBMIT)
                // {

                //     dt.Rows.Remove(dt.Rows[0]);

                //     var fileUploadHistory = Command.UnitOfWork.ProductCodeSheetUploadHistoryRepository.Where(new { CreatedBy = UserRoleId, ProductCodeSheetUploadStatusID = (short)ProductCodeSheetUploadStatusEnum.DATA_VALIDATED }).LastOrDefault();
                //     var cts = new CancellationTokenSource();
                //     CancellationToken token = cts.Token;

                //     try
                //     {
                //         ProcessRequestAsyc(dt, filePath, fileUploadHistory.AttachedFileID, RequestDTO, propertyNameList, Command.CurrentUserName, UserRoleId, token, cts);
                //         // Task.Factory.StartNew(
                //         //     async () => await ProcessRequestAsyc(dt, filePath, fileUploadHistory.AttachedFileID, RequestDTO, propertyNameList, Command.CurrentUserName, UserRoleId, token, cts)
                //         //     , token
                //         //     , TaskCreationOptions.LongRunning
                //         //     , TaskScheduler.Current);

                //     }
                //     catch (OperationCanceledException ex)
                //     {
                //         Log.Error("[{0}.{1}] {2}-{3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex, ex.StackTrace);
                //     }
                //     finally
                //     {
                //         cts.Dispose();
                //     }
                //     ResponseDTO.GridColumns = GetGridColumns(RequestDTO.ActionID);
                //     ResponseDTO.Data = GetRegisteredRecords(dispuedTable);



                //     ResponseDTO.DisputedRecordCount = dispuedTable.Rows.Count;
                //     ResponseDTO.DuplicateRecordCount = duplicateTable.Rows.Count;
                //     ResponseDTO.TotalRecordCount = dt.Rows.Count;
                //     ResponseDTO.ProcessedRecordsCount = fileUploadHistory.ProcessedRecordsCount == null ? 0 : fileUploadHistory.ProcessedRecordsCount;

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


                // var columnError = new GridColumns
                // {
                //     Field = "error",
                //     Title = "Error",
                //     Editor = "string",
                //     Width = "400px"
                // };
                // gridColumns.Add(columnError);

                // var columnErrorRowIndex = new GridColumns
                // {
                //     Field = "rowIndex",
                //     Title = "Row Index",
                //     Editor = "string",
                //     Width = "20px"
                // };
                // gridColumns.Add(columnErrorRowIndex);



                return gridColumns;

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        // private async Task ProcessRequestAsyc(DataTable dt, string filePath, long attachedFileID, UploadConfigrationFileRequestDTO request, List<SheetAttributeMapping> propertyNameList, string userName, int userRoleId, CancellationToken token, CancellationTokenSource cts)
        // {

        //     Log.Information("[{0}.{1}] Request Started.", this.GetType().Name, MethodBase.GetCurrentMethod().Name);

        //     try
        //     {
        //         string connectionString = Utility.DecryptConnectionString();
        //         Log.Information($"UploadFileStrategy: Connectstring: {connectionString}");
        //         using (UnitOfWork uow = new UnitOfWork(connectionString))
        //         {
        //             MapandInsertDataTable(uow, dt, request, propertyNameList, attachedFileID, token, cts, userRoleId);

        //         }
        //     }
        //     catch (System.Exception ex)
        //     {
        //         string connectionString = "Server=10.1.4.58;Initial Catalog=ITT;User ID=psw_app;Password=@Password1;";
        //         Log.Information($"UploadFileStrategy: Connectstring: {connectionString}");
        //         Log.Error("[{0}.{1}] {2}-{3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex, ex.StackTrace);
        //         using (UnitOfWork uow = new UnitOfWork(connectionString))
        //         {
        //             // if (ex is ObjectDisposedException)
        //             // {
        //             //     UpdateFileUploadHistory(uow, fileUploadHistoryID, (int)ProductCodeSheetUploadStatusEnum.CANCELLED, userRoleId);
        //             // }
        //             // else
        //             // {
        //             UpdateFileUploadHistory(uow, attachedFileID, (int)ProductCodeSheetUploadStatusEnum.FAILED, userRoleId);
        //             // }
        //         }
        //         throw ex;
        //     }
        // }

        // private void MapandInsertDataTable(UnitOfWork uow, DataTable dt, UploadConfigrationFileRequestDTO request, List<SheetAttributeMapping> propertyNameList, long attachedFileID, CancellationToken token, CancellationTokenSource cts, int userRoleId)
        // {
        //     Log.Information($"UploadNationalRegisterStrategy | MapandInsertDataTable | dt rows: {dt.Rows.Count}");

        //     var duplicateRecordsCount = 0;
        //     var totalRecordsCount = dt.Rows.Count;
        //     var fileUploadHistory = Command.UnitOfWork.ProductCodeSheetUploadHistoryRepository.Where(new { AttachedFileID = attachedFileID, CreatedBy = UserRoleId }).LastOrDefault();


        //     duplicateRecordsCount = totalRecordsCount - dt.Rows.Count;
        //     for (int i = 0; i < dt.Rows.Count; i++)
        //     {
        //         if (i == 0)
        //         {
        //             var fileUploadHistoryAdd = new ProductCodeSheetUploadHistory
        //             {

        //                 AttachedFileID = attachedFileID,
        //                 Name = fileUploadHistory.Name,
        //                 AgencyID = RequestDTO.AgencyID,
        //                 TotalRecordsCount = dt.Rows.Count,
        //                 ProcessedRecordsCount = dt.Rows.Count,
        //                 DuplicateRecordsCount = 0,

        //                 TradeTranTypeID = RequestDTO.TradeTranTypeID,
        //                 IsCurrent = true,
        //                 DisputedRecordsCount = 0,
        //                 ProductCodeSheetUploadStatusID = (short)ProductCodeSheetUploadStatusEnum.IN_PROGRESS,
        //                 CreatedBy = UserRoleId,
        //                 UpdatedBy = UserRoleId,
        //                 CreatedOn = DateTime.Now,
        //                 UpdatedOn = DateTime.Now
        //             };


        //             long fileUploadHistoryID = Command.UnitOfWork.ProductCodeSheetUploadHistoryRepository.Add(fileUploadHistoryAdd);
        //         }

        //         DataRow row = dt.Rows[i];
        //         InsertProductCodeRecord(uow, row, (Int16)ProductCodeSheetUploadStatusEnum.PROCESSED, request, fileUploadHistory.ID, propertyNameList, userRoleId);

        //         UpdateFileHistory(uow, fileUploadHistory.AttachedFileID, token, cts, totalRecordsCount, i, userRoleId);
        //     }

        // }
        // private void UpdateFileHistory(UnitOfWork uow, long attachedFileID, CancellationToken token, CancellationTokenSource cts, long totalCount, int processedRecordsCount, int userRoleId)
        // {
        //     try
        //     {

        //         var fileUploadHistory = uow.ProductCodeSheetUploadHistoryRepository.Where(new { AttachedFileID = attachedFileID, CreatedBy = UserRoleId }).LastOrDefault();
        //         // var fileUploadHistory = uow.ProductCodeSheetUploadHistoryRepository.Where(new { ID = attachedFileID }).FirstOrDefault();//.Get(fileUploadHistoryID.ToString());

        //         // Stop operation if cancellation is requested
        //         // if (fileUploadHistory.ProductCodeSheetUploadStatusID == (int)ProductCodeSheetUploadStatusEnum.CANCELLED)
        //         // {
        //         //     cts.Cancel();
        //         //     if (cts.IsCancellationRequested)
        //         //     {
        //         //         Log.Warning("[{0}.{1}] File processing has been cancelled", this.GetType().Name, MethodBase.GetCurrentMethod().Name, attachedFileID);
        //         //         token.ThrowIfCancellationRequested();
        //         //     }
        //         // }

        //         fileUploadHistory.ProcessedRecordsCount = processedRecordsCount + 1;
        //         // fileUploadHistory.DisputedRecordsCount = fileUploadHistory.DisputedRecordsCount + 1;
        //         fileUploadHistory.UpdatedOn = DateTime.Now;

        //         if ((fileUploadHistory.TotalRecordsCount) == processedRecordsCount + 1)
        //         {

        //             fileUploadHistory.ProcessingResponse = "File Successfully Uploaded.";
        //             fileUploadHistory.ProductCodeSheetUploadStatusID = (int)ProductCodeSheetUploadStatusEnum.PROCESSED;
        //         }

        //         uow.ProductCodeSheetUploadHistoryRepository.Update(fileUploadHistory);

        //         // if ((fileUploadHistory.TotalRecordsCount) == processedRecordsCount + 1)
        //         // {
        //         //     SendMessage(fileUploadHistory, userRoleId, InboxRequestType.FILE_UPLOAD_SUCCESS);
        //         // }

        //     }
        //     catch (System.Exception ex)
        //     {
        //         Log.Error("[{0}.{1}] {2}-{3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex, ex.StackTrace);
        //         throw ex;
        //     }
        // }
        // private void SendMessage(ProductCodeSheetUploadHistory fileUploadHistory, int userRoleId, InboxRequestType requestType)
        // {
        //     Log.Information($"| Strategy Name : {StrategyName} || Method ID : {MethodID} | Method Name : {MethodBase.GetCurrentMethod().Name} | Starting Method ");
        //     List<int> userRoleIds = new List<int>();
        //     userRoleIds.Add(userRoleId);
        //     Dictionary<string, string> placeHolders = SetPlaceHolders(fileUploadHistory);
        //     var requestDto = new SendInboxMessageRequestDTO
        //     {
        //         FromUserRoleId = userRoleId,
        //         ToUserRoleIds = userRoleIds,
        //         InboxRequestTypeId = (byte)requestType,
        //         Placeholders = placeHolders
        //     };
        //     Log.Information($"| Strategy Name : {StrategyName} || Method ID : {MethodID} | Method Name : {MethodBase.GetCurrentMethod().Name} | Sending Notification Message.");
        //     Messenger.SendMessage(Command, requestDto);
        //     Log.Information("|{StrategyName}|{MethodID}| Notification Request DTO: {@RequestDTO}", StrategyName, MethodID, requestDto);
        //     Log.Information($"| Strategy Name : {StrategyName} || Method ID : {MethodID} | Method Name : {MethodBase.GetCurrentMethod().Name} | Ending Method ");
        // }
        // public Dictionary<string, string> SetPlaceHolders(ProductCodeSheetUploadHistory fileUploadHistory)
        // {
        //     var placeholders = new Dictionary<string, string>();
        //     placeholders.Add("@FileName", fileUploadHistory.Name);
        //     placeholders.Add("@TotalRecordsCount", fileUploadHistory.TotalRecordsCount.ToString());
        //     placeholders.Add("@ProcessedRecordsCount", fileUploadHistory.ProcessedRecordsCount.ToString());
        //     return placeholders;
        // }
        // public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        // {
        //     // ExpandoObject supports IDictionary so we can extend it like this
        //     var expandoDict = expando as IDictionary<string, object>;
        //     if (expandoDict.ContainsKey(propertyName))
        //         expandoDict[propertyName] = propertyValue;
        //     else
        //         expandoDict.Add(propertyName, propertyValue);
        // }
        // private void InsertProductCodeRecord(UnitOfWork uow, DataRow Row, short status, UploadConfigrationFileRequestDTO request, long fileUploadHistoryID, List<SheetAttributeMapping> propertyNameList, int userRoleId)
        // {
        //     string productCode = Row["Product Code"].ToString();
        //     string hsCode = Row["HSCode"].ToString();
        //     var getFactor = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType == (short)FileTypeEnum.UPDATE_REGULATIONS_TEMPLATE ? (short)FileTypeEnum.ADD_REGULATIONS_TEMPLATE : RequestDTO.FileType).Where(x => x.CheckDuplicate == true).ToList();
        //     getFactor.RemoveAll(x => x.NameLong.Contains("HSCode"));
        //     getFactor.RemoveAll(x => x.NameLong.Contains("Product Code"));
        //     string factor = Row[getFactor.FirstOrDefault().NameLong].ToString();


        //     dynamic obj = new ExpandoObject();
        //     var productCodeAgencyLink = uow.ProductCodeEntityRepository.GetProductCodeValidity(productCode, request.AgencyID, request.TradeTranTypeID);

        //     foreach (var column in propertyNameList)
        //     {
        //         AddProperty(obj, column.NameShort, Row[column.Index - 1].ToString() ?? "");

        //     }
        //     if (RequestDTO.FileType == (short)FileTypeEnum.UPDATE_REGULATIONS_TEMPLATE)
        //     {
        //         var lpcoRegulationUpdate = uow.LPCORegulationRepository.Where(new { AgencyID = request.AgencyID, TradeTranTypeID = request.TradeTranTypeID, HSCode = hsCode, HSCodeExt = productCode, Factor = factor }).LastOrDefault();
        //         lpcoRegulationUpdate.EffectiveThruDt = DateTime.Now;
        //         uow.LPCORegulationRepository.Update(lpcoRegulationUpdate);
        //     }
        //     else if (RequestDTO.FileType == (short)FileTypeEnum.INACTIVATE_REGULATIONS_TEMPLATE)
        //     {
        //         string expiryDate = Row["Expiry Date"].ToString();

        //         var lpcoRegulationUpdate = uow.LPCORegulationRepository.Where(new { AgencyID = request.AgencyID, TradeTranTypeID = request.TradeTranTypeID, HSCode = hsCode, HSCodeExt = productCode, Factor = factor }).LastOrDefault();
        //         lpcoRegulationUpdate.EffectiveThruDt = String.IsNullOrEmpty(expiryDate) ? DateTime.Now : Convert.ToDateTime(expiryDate);
        //         uow.LPCORegulationRepository.Update(lpcoRegulationUpdate);
        //     }
        //     if (RequestDTO.FileType != (short)FileTypeEnum.INACTIVATE_REGULATIONS_TEMPLATE)
        //     {
        //         LPCORegulation lpcoRegulation = new LPCORegulation
        //         {
        //             AgencyID = request.AgencyID,
        //             RegulationJson = System.Text.Json.JsonSerializer.Serialize(obj),// JsonSerializer.Serialize<dynamic>(Row.ItemArray),
        //             CreatedOn = DateTime.Now,
        //             UpdatedOn = DateTime.Now,
        //             CreatedBy = userRoleId,
        //             UpdatedBy = userRoleId,
        //             ProductCodeAgencyLinkID = productCodeAgencyLink.LastOrDefault().ID,
        //             TradeTranTypeID = request.TradeTranTypeID,
        //             EffectiveFromDt = DateTime.Now,
        //             EffectiveThruDt = productCodeAgencyLink.LastOrDefault().EffectiveThruDt,
        //             HSCode = hsCode,
        //             HSCodeExt = productCode,
        //             Factor = factor
        //         };
        //         var lpcoRegulationId = uow.LPCORegulationRepository.Add(lpcoRegulation);
        //     }



        //     // ProductCodeEntity productCodeEntity = new ProductCodeEntity();

        //     // try
        //     // {
        //     //     var ChapterCode = Row[0].ToString().Substring(0, 2);
        //     //     var ProductCodeChapter = uow.ProductCodeChapterRepository.Where(new { Code = ChapterCode }).FirstOrDefault();
        //     //     if(ProductCodeChapter!= null)
        //     //     {
        //     //         productCodeEntity.HSCode = Row[0].ToString();
        //     //         productCodeEntity.HSCodeExt = Row[0].ToString() + "." + Row[1].ToString();
        //     //         productCodeEntity.ProductCode = Row[1].ToString();
        //     //         productCodeEntity.TradeTranTypeID = Convert.ToInt16(Row[3]);
        //     //         productCodeEntity.ChapterCode = ChapterCode;
        //     //         productCodeEntity.ProductCodeChapterID = (short)ProductCodeChapter.ID;
        //     //         productCodeEntity.Description = Row[2].ToString();
        //     //         productCodeEntity.ProductCodeSheetUploadHistoryID = fileUploadHistoryID;
        //     //         productCodeEntity.EffectiveFromDt = Convert.ToDateTime(Row[4].ToString());
        //     //         productCodeEntity.EffectiveThruDt = (String.IsNullOrEmpty(Row[5].ToString()) || 
        //     //                                              String.IsNullOrWhiteSpace(Row[5].ToString())) ? DateTime.MaxValue : Convert.ToDateTime(Row[5].ToString()).AddHours(23).AddMinutes(59).AddSeconds(59) ;

        //     //         productCodeEntity.CreatedOn = DateTime.Now;
        //     //         productCodeEntity.UpdatedOn = DateTime.Now;
        //     //         productCodeEntity.CreatedBy = userRoleId;
        //     //         productCodeEntity.UpdatedBy = userRoleId;
        //     //         // ACRHeader.CompletedOn = DateTime.Now;

        //     //         var productCodeEntityId = uow.ProductCodeEntityRepository.Add(productCodeEntity);
        //     //     }
        //     //     else{
        //     //         Log.Information("[{0}.{1}] Product Code Chapter Not Found {2} ", this.GetType().Name, MethodBase.GetCurrentMethod().Name, ProductCodeChapter);
        //     //         throw new NullReferenceException(" Product Code Chapter Not Found ");

        //     //     }
        //     // }
        //     // catch (System.Exception ex)
        //     // {
        //     //     Log.Error("[{0}.{1}] {2}-{3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex, ex.StackTrace);
        //     //     throw ex;
        //     // }
        // }
        // private bool UpdateFileUploadHistory(UnitOfWork uow, long fileUploadHistoryID, short statusId, int userRoleId)
        // {
        //     try
        //     {
        //         Log.Information("[{0}.{1}] Updating File History for file id: {2} as {3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, fileUploadHistoryID, statusId);

        //         var fileUploadHistory = uow.ProductCodeSheetUploadHistoryRepository.Get(fileUploadHistoryID.ToString());
        //         fileUploadHistory.ProductCodeSheetUploadStatusID = statusId;
        //         fileUploadHistory.UpdatedOn = DateTime.Now;
        //         fileUploadHistory.UpdatedBy = userRoleId;

        //         uow.ProductCodeSheetUploadHistoryRepository.Update(fileUploadHistory);

        //         Log.Information("[{0}.{1}] Updated File History for file id: {2} as {3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, fileUploadHistoryID, statusId);

        //         return true;
        //     }
        //     catch (System.Exception ex)
        //     {
        //         Log.Error("[{0}.{1}] {2}-{3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex, ex.StackTrace);
        //         throw ex;
        //     }
        // }

        // public DataTable GetDuplicateRecords(DataTable dt, DataTable duplicateTable, int errorColumnPosition)
        // {
        //     Hashtable hTable = new Hashtable();
        //     ArrayList duplicateList = new ArrayList();

        //     var duplicateCheckIndexList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType == (short)FileTypeEnum.UPDATE_REGULATIONS_TEMPLATE ? (short)FileTypeEnum.ADD_REGULATIONS_TEMPLATE : RequestDTO.FileType).Where(x => x.CheckDuplicate == true).Select(x => x.Index).ToList();
        //     //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
        //     //And add duplicate item value in arraylist.
        //     var indexString = "";

        //     foreach (DataRow drow in dt.Rows)
        //     {
        //         foreach (int a in duplicateCheckIndexList)
        //         {
        //             indexString += drow[a - 1].ToString();

        //         }
        //         if (hTable.Contains(indexString))
        //             duplicateList.Add(drow);
        //         else
        //             hTable.Add(indexString, string.Empty);
        //         indexString = "";
        //     }
        //     int rowIndex = 0;

        //     //Removing a list of duplicate items from datatable.
        //     foreach (DataRow dRow in duplicateList)
        //     {
        //         rowIndex += 1;
        //         DataRow row = duplicateTable.NewRow();
        //         row.ItemArray = dRow.ItemArray;

        //         row[errorColumnPosition] = "Duplicate Row";
        //         row[errorColumnPosition + 1] = rowIndex + 1;
        //         duplicateTable.Rows.Add(row);
        //     }

        //     //Datatable which contains unique records will be return as output.
        //     return duplicateTable;

        // }
        // public DataRow AddToDisputedTable(DataTable dispuedTable, DataRow d, int hsCode, int productCode)
        // {
        //     var ifNotExist = 0;

        //     DataRow row = dispuedTable.NewRow();
        //     if (dispuedTable.Rows.Count == 0)
        //     {
        //         row.ItemArray = d.ItemArray;
        //         return row;
        //     }
        //     foreach (DataRow i in dispuedTable.Rows)
        //     {
        //         if (i[productCode] == d[productCode].ToString() && i[hsCode] == d[hsCode].ToString())
        //         {
        //             ifNotExist += 1;
        //         }

        //     }
        //     if (ifNotExist <= 0)
        //     {
        //         row.ItemArray = d.ItemArray;

        //         // dispuedTable.Rows.Add(row);
        //     }

        //     return row;
        // }


        private IDictionary<string, string> IsFileColumnsCorrect(List<string> headerRow,List<string> propertyNameList)
        {
            // List<string> dbColumns = this.Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType == (short)FileTypeEnum.UPDATE_REGULATIONS_TEMPLATE ? (short)FileTypeEnum.ADD_REGULATIONS_TEMPLATE : RequestDTO.FileType).OrderBy(x => x.Index).Select(x => x.NameLong).ToList();//.Where(new { TradeTranTypeID = RequestDTO.TradeTranTypeID, AgencyID = RequestDTO.AgencyID, SheetType = RequestDTO.FileType, isActive = true }).OrderBy(x => x.Index).Select(x => x.NameLong).ToList();
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