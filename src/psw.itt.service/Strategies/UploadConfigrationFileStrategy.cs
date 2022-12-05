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

namespace PSW.ITT.Service.Strategies
{
    public class UploadConfigrationFileStrategy : ApiStrategy<UploadConfigrationFileRequestDTO, UploadConfigrationFileResponseDTO>
    {
        #region Constructors

        /// <summary>
        /// Strategy Constructor
        /// </summary>
        /// <param name="request">Takes CommandRequest</param>
        public UploadConfigrationFileStrategy(CommandRequest request) : base(request)
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

                if (currentRole == null || (currentRole.RoleCode != RoleCode.TRADER && currentRole.RoleCode != RoleCode.OGA_ITT_OFFICER))
                {
                    Log.Information("[{0}.{1}] Invalid Role", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return BadRequestReply("Invalid Role");
                }

                UserRoleId = currentRole.UserRoleID;

                // Check if previous upload is in progress

                var CurrentFileUploadHistory = Command.UnitOfWork.ProductCodeSheetUploadHistoryRepository.Where(new { AgencyID = RequestDTO.AgencyID, TradeTranTypeID = RequestDTO.TradeTranTypeID }).LastOrDefault();

                if (CurrentFileUploadHistory != null && CurrentFileUploadHistory.ProductCodeSheetUploadStatusID == (int)ProductCodeSheetUploadStatusEnum.IN_PROGRESS)
                {
                    Log.Information("[{0}.{1}] Please wait, The file is already being processed", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return BadRequestReply("Please wait, The file is already being processed.");
                }

                Command.UnitOfWork.ProductCodeSheetUploadHistoryRepository.SetIsCurrent(RequestDTO.AgencyID);

                DataTable dt = new DataTable();
                ResponseDTO = new UploadConfigrationFileResponseDTO();

                var filePath = Utility.AESDecrypt256(RequestDTO.FilePath);
                dt = GetDataTableFromExcel(filePath);
                var dispuedTable = new DataTable();
                var duplicateTable = new DataTable();
                var formatTable = new DataTable();
                var errorColumnPosition = 0;
                var errorColumnIndexPosition = 0;
                errorColumnPosition = dt.Rows[0].ItemArray.Length;
                errorColumnIndexPosition = dt.Rows[0].ItemArray.Length + 1;
                foreach (var cols in dt.Rows[0].ItemArray)
                {
                    dispuedTable.Columns.Add(cols.ToString(), typeof(string));
                    duplicateTable.Columns.Add(cols.ToString(), typeof(string));
                    formatTable.Columns.Add(cols.ToString(), typeof(string));

                }
                dispuedTable.Columns.Add("Error", typeof(string));
                duplicateTable.Columns.Add("Error", typeof(string));
                duplicateTable.Columns.Add("Row Index", typeof(string));
                dispuedTable.Columns.Add("Row Index", typeof(string));
                dt.AcceptChanges();
                if (dt.Rows.Count < 1)
                {
                    Log.Information("[{0}.{1}] File not Uploaded Successfully as you are trying to upload an empty File", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return BadRequestReply("File not Uploaded Successfully as you are trying to upload an empty File.");
                }

                var fileCheck = IsFileColumnsCorrect((dt.Rows[0].ItemArray).Select(x => x.ToString()).ToList());

                if (RequestDTO.ActionID == (short)ActionID.VALIDATE)
                {

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
                        AgencyID = RequestDTO.AgencyID,
                        TradeTranTypeID = RequestDTO.TradeTranTypeID,
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
                        ResponseDTO.GridColumns = GetGridFormatColumns(dt.Rows[0].ItemArray.ToList(), processingResponse);
                        ResponseDTO.Data = GetRegisteredRecords(new DataTable());
                        ResponseDTO.DisputedRecordCount = 0;
                        ResponseDTO.StatusID = validationStatus;
                        ResponseDTO.DuplicateRecordCount = 0;
                        ResponseDTO.TotalRecordCount = dt.Rows.Count;
                        ResponseDTO.ProcessedRecordsCount = 0;
                        Log.Information($"| Strategy Name : {StrategyName} | Method ID : {MethodID} | File Structure Validation Failed.");
                        return BadRequestReply("Validation Failed.");

                    }

                    dt.Rows.Remove(dt.Rows[0]);


                    //TODO will uncomment
                    // var columnsCheck = CheckIsMandatoryColumnsAvailable(dt);

                    // if (columnsCheck != "")
                    // {
                    //     Log.Information("[{0}.{1}] Error in File, Column {2} could not be null.", this.GetType().Name, MethodBase.GetCurrentMethod().Name, columnsCheck);
                    //     return BadRequestReply($"Error in File, Column {columnsCheck} could not be null.");

                    // }

                    if (RequestDTO.ActionID == (short)RegulationAction.ADD_REGULATION)
                    {

                    }
                    else if (RequestDTO.ActionID == (short)RegulationAction.UPDATE_REGULATION)
                    {


                    }
                    else if (RequestDTO.ActionID == (short)RegulationAction.INACTIVATE_REGULATION)
                    {

                    }



                    duplicateTable = GetDuplicateRecords(dt, duplicateTable, errorColumnPosition);
                    var activeProductCodes = Command.UnitOfWork.ProductCodeEntityRepository.GetActiveAgencyProductCode(RequestDTO.AgencyID);
                    var propertyNameList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType).ToList();
                    var validationList = Command.UnitOfWork.AttributeValidationMappingRepository.GetAssociatedValidationList(propertyNameList.Select(x => x.ID).ToList()).ToList();
                    int rowIndex = 0;

                    foreach (DataRow d in dt.Rows)
                    {

                        string error = "";
                        DataRow row = dispuedTable.NewRow();
                        for (var i = 0; i < d.ItemArray.Count(); i++)
                        {
                            List<Data.DTO.ProductCodeValidationList> validation = validationList.Where(x => x.Index == i + 1).ToList();
                            if (validation.Count > 0)
                            {
                                ProductCodeFileValidation PCValidator = new ProductCodeFileValidation(d.ItemArray[i].ToString(), dt.Columns[i].ToString(), validation, Command, RequestDTO.AgencyID, RequestDTO.TradeTranTypeID);
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
                        var duplicateCheckIndexList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType).Where(x => x.CheckDuplicate == true).Select(x => x.Index).ToList();

                        foreach (DataRow drow in duplicateTable.Rows)
                        {
                            var count = 0;
                            foreach (int a in duplicateCheckIndexList)
                            {
                                if (drow[a - 1].ToString() == d[a - 1].ToString()) count++;
                            }
                            if (count == duplicateCheckIndexList.Count)
                            {
                                error = error == "" ? drow[errorColumnPosition].ToString() : String.Concat(error, ", ", drow[errorColumnPosition]);
                            }
                        }
                        if (error != "")
                        {

                            row.ItemArray = d.ItemArray;
                            row[errorColumnPosition] = error;
                            row[errorColumnIndexPosition] = rowIndex + 1;
                            dispuedTable.Rows.Add(row);
                        }
                    }
                    var duplicateRecordCount = dt.Rows.Count - duplicateTable.Rows.Count;
                    // DataTable mergeDuplicateAndDisputedTable = new DataTable();
                    // mergeDuplicateAndDisputedTable.Merge(dispuedTable);
                    // mergeDuplicateAndDisputedTable.Merge(duplicateTable);
                    short status = (short)ProductCodeSheetUploadStatusEnum.DATA_VALIDATED;
                    // ValidateNationalRegistration validateRegistrationData = new ValidateNationalRegistration();

                    // Update National RegisterFileHistory
                    var fileUploadHistory = new ProductCodeSheetUploadHistory();
                    if (duplicateTable.Rows.Count > 0 || dispuedTable.Rows.Count > 0)
                    {
                        status = (short)ProductCodeSheetUploadStatusEnum.DATA_VALIDATION_FAILED;
                    }
                    fileUploadHistory.AttachedFileID = RequestDTO.FileID;
                    fileUploadHistory.Name = RequestDTO.FileName;
                    fileUploadHistory.AgencyID = RequestDTO.AgencyID;
                    fileUploadHistory.TotalRecordsCount = dt.Rows.Count;
                    fileUploadHistory.ProcessedRecordsCount = dt.Rows.Count;
                    fileUploadHistory.DuplicateRecordsCount = duplicateTable.Rows.Count;
                    fileUploadHistory.IsCurrent = true;
                    fileUploadHistory.DisputedRecordsCount = dispuedTable.Rows.Count;
                    fileUploadHistory.ProductCodeSheetUploadStatusID = status;
                    fileUploadHistory.CreatedBy = UserRoleId;
                    fileUploadHistory.UpdatedBy = UserRoleId;
                    fileUploadHistory.CreatedOn = DateTime.Now;
                    fileUploadHistory.UpdatedOn = DateTime.Now;

                    long fileUploadHistoryID = Command.UnitOfWork.ProductCodeSheetUploadHistoryRepository.Add(fileUploadHistory);
                    var fileUploadHistoryRecord = Command.UnitOfWork.ProductCodeSheetUploadHistoryRepository.Where(new { ID = fileUploadHistoryID }).FirstOrDefault();
                    // ResponseDTO = new UploadConfigrationFileResponseDTO();
                    if (fileUploadHistoryID < 1)
                    {
                        Log.Information("[{0}.{1}] Error in File uploading.", this.GetType().Name, MethodBase.GetCurrentMethod().Name); //columnsCheck
                        return BadRequestReply($"Error in File uploading, Please try again later.");
                    }
                    if (dispuedTable.Rows.Count <= 0)
                    {

                        // var cts = new CancellationTokenSource();
                        // CancellationToken token = cts.Token;

                        // try
                        // {
                        //     Task.Factory.StartNew(
                        //         async () => await ProcessRequestAsyc(dt, filePath, fileUploadHistoryID, RequestDTO, propertyNameList, Command.CurrentUserName, UserRoleId, token, cts)
                        //         , token
                        //         , TaskCreationOptions.LongRunning
                        //         , TaskScheduler.Current);

                        // }
                        // catch (OperationCanceledException ex)
                        // {
                        //     Log.Error("[{0}.{1}] {2}-{3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex, ex.StackTrace);
                        // }
                        // finally
                        // {
                        //     cts.Dispose();
                        // }
                        ResponseDTO.DisputedRecordCount = dispuedTable.Rows.Count;
                        ResponseDTO.DuplicateRecordCount = duplicateTable.Rows.Count;
                        ResponseDTO.TotalRecordCount = dt.Rows.Count;
                        ResponseDTO.GridColumns = GetGridColumns(RequestDTO.ActionID);
                        ResponseDTO.Data = GetRegisteredRecords(dispuedTable);
                        ResponseDTO.ProcessedRecordsCount = fileUploadHistoryRecord.ProcessedRecordsCount == null ? 0 : fileUploadHistoryRecord.ProcessedRecordsCount;
                        Log.Information($"| Strategy Name : {StrategyName} | Method ID : {MethodID} | Upload Successfully");
                        return OKReply("Upload Successfully.");

                    }


                    ResponseDTO.GridColumns = GetGridColumns(RequestDTO.ActionID);
                    ResponseDTO.Data = GetRegisteredRecords(dispuedTable);



                    ResponseDTO.DisputedRecordCount = dispuedTable.Rows.Count;
                    ResponseDTO.DuplicateRecordCount = duplicateTable.Rows.Count;
                    ResponseDTO.TotalRecordCount = dt.Rows.Count;
                    ResponseDTO.ProcessedRecordsCount = fileUploadHistoryRecord.ProcessedRecordsCount == null ? 0 : fileUploadHistoryRecord.ProcessedRecordsCount; ;

                    if (dispuedTable.Rows.Count > 0)
                    {
                        return BadRequestReply("Validation Failed.");
                    }
                    else
                    {

                        return OKReply("Validated Successfully.");
                    }

                    // return OKReply("Upload Successfully.");
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
        private List<dynamic> GetRegisteredRecords(DataTable dt)
        {
            try
            {
                List<dynamic> gridData = new List<dynamic>();


                var propertyNameList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType).OrderBy(x => x.Index).ToList();


                int rowIndex = 0;
                foreach (DataRow drow in dt.Rows)
                {
                    rowIndex += 1;
                    IDictionary<string, object> expandoDict = new ExpandoObject();
                    foreach (var x in propertyNameList)
                    {
                        expandoDict.Add(x.NameShort, drow[x.Index - 1]);

                    }
                    if (RequestDTO.ActionID == (short)ActionID.VALIDATE)
                    {
                        expandoDict.Add("error", drow[propertyNameList.Count]);
                        expandoDict.Add("rowIndex", rowIndex + 1);
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

        private List<GridColumns> GetGridFormatColumns(List<object> columnNames, string processingResponse)
        {
            try
            {
                List<GridColumns> gridColumns = new List<GridColumns>();

                var propertyNameList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType).OrderBy(x => x.Index).ToList();
                foreach (var x in columnNames)
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
        private List<GridColumns> GetGridColumns(short actionID)
        {
            try
            {
                List<GridColumns> gridColumns = new List<GridColumns>();

                var propertyNameList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType).OrderBy(x => x.Index).ToList();
                foreach (var x in propertyNameList)
                {
                    var column = new GridColumns();
                    column.Field = x.NameShort;
                    column.Title = x.NameLong;
                    column.Editor = "string";
                    column.Width = "90px";
                    gridColumns.Add(column);
                }


                var columnError = new GridColumns
                {
                    Field = "error",
                    Title = "Error",
                    Editor = "string",
                    Width = "400px"
                };
                gridColumns.Add(columnError);

                var columnErrorRowIndex = new GridColumns
                {
                    Field = "rowIndex",
                    Title = "Row Index",
                    Editor = "string",
                    Width = "20px"
                };
                gridColumns.Add(columnErrorRowIndex);



                return gridColumns;

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
        #region Methods  
        private async Task ProcessRequestAsyc(DataTable dt, string filePath, long fileUploadHistoryID, UploadConfigrationFileRequestDTO request, List<SheetAttributeMapping> propertyNameList, string userName, int userRoleId, CancellationToken token, CancellationTokenSource cts)
        {

            Log.Information("[{0}.{1}] Request Started.", this.GetType().Name, MethodBase.GetCurrentMethod().Name);

            try
            {
                string connectionString = Utility.DecryptConnectionString();
                Log.Information($"UploadFileStrategy: Connectstring: {connectionString}");
                using (UnitOfWork uow = new UnitOfWork(connectionString))
                {
                    MapandInsertDataTable(uow, dt, request, propertyNameList, fileUploadHistoryID, token, cts, userRoleId);

                }
            }
            catch (System.Exception ex)
            {
                string connectionString = "Server=10.1.4.58;Initial Catalog=ITT;User ID=psw_app;Password=@Password1;";
                Log.Information($"UploadFileStrategy: Connectstring: {connectionString}");
                Log.Error("[{0}.{1}] {2}-{3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex, ex.StackTrace);
                using (UnitOfWork uow = new UnitOfWork(connectionString))
                {
                    if (ex is ObjectDisposedException)
                    {
                        UpdateFileUploadHistory(uow, fileUploadHistoryID, (int)ProductCodeSheetUploadStatusEnum.CANCELLED, userRoleId);
                    }
                    else
                    {
                        UpdateFileUploadHistory(uow, fileUploadHistoryID, (int)ProductCodeSheetUploadStatusEnum.FAILED, userRoleId);
                    }
                }
                throw ex;
            }
        }

        private void MapandInsertDataTable(UnitOfWork uow, DataTable dt, UploadConfigrationFileRequestDTO request, List<SheetAttributeMapping> propertyNameList, long fileUploadHistoryID, CancellationToken token, CancellationTokenSource cts, int userRoleId)
        {
            Log.Information($"UploadNationalRegisterStrategy | MapandInsertDataTable | dt rows: {dt.Rows.Count}");

            var duplicateRecordsCount = 0;
            var totalRecordsCount = dt.Rows.Count;


            duplicateRecordsCount = totalRecordsCount - dt.Rows.Count;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                InsertProductCodeRecord(uow, row, (Int16)ProductCodeSheetUploadStatusEnum.PROCESSED, request, fileUploadHistoryID, propertyNameList, userRoleId);

                UpdateFileHistory(uow, fileUploadHistoryID, token, cts, totalRecordsCount, i, userRoleId);
            }

        }
        private void UpdateFileHistory(UnitOfWork uow, long fileUploadHistoryID, CancellationToken token, CancellationTokenSource cts, long totalCount, int processedRecordsCount, int userRoleId)
        {
            try
            {
                var fileUploadHistory = uow.ProductCodeSheetUploadHistoryRepository.Get(fileUploadHistoryID.ToString());

                // Stop operation if cancellation is requested
                if (fileUploadHistory.ProductCodeSheetUploadStatusID == (int)ProductCodeSheetUploadStatusEnum.CANCELLED)
                {
                    cts.Cancel();
                    if (cts.IsCancellationRequested)
                    {
                        Log.Warning("[{0}.{1}] File processing has been cancelled", this.GetType().Name, MethodBase.GetCurrentMethod().Name, fileUploadHistoryID);
                        token.ThrowIfCancellationRequested();
                    }
                }

                fileUploadHistory.ProcessedRecordsCount = processedRecordsCount + 1;
                // fileUploadHistory.DisputedRecordsCount = fileUploadHistory.DisputedRecordsCount + 1;
                fileUploadHistory.UpdatedOn = DateTime.Now;

                if ((fileUploadHistory.TotalRecordsCount) == processedRecordsCount + 1)
                {
                    fileUploadHistory.ProductCodeSheetUploadStatusID = (int)ProductCodeSheetUploadStatusEnum.PROCESSED;
                }

                uow.ProductCodeSheetUploadHistoryRepository.Update(fileUploadHistory);
                if ((fileUploadHistory.TotalRecordsCount) == processedRecordsCount + 1)
                {
                    SendMessage(fileUploadHistory, userRoleId, InboxRequestType.FILE_UPLOAD_SUCCESS);
                }

            }
            catch (System.Exception ex)
            {
                Log.Error("[{0}.{1}] {2}-{3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex, ex.StackTrace);
                throw ex;
            }
        }
        private void SendMessage(ProductCodeSheetUploadHistory fileUploadHistory, int userRoleId, InboxRequestType requestType)
        {
            Log.Information($"| Strategy Name : {StrategyName} || Method ID : {MethodID} | Method Name : {MethodBase.GetCurrentMethod().Name} | Starting Method ");
            List<int> userRoleIds = new List<int>();
            userRoleIds.Add(userRoleId);
            Dictionary<string, string> placeHolders = SetPlaceHolders(fileUploadHistory);
            var requestDto = new SendInboxMessageRequestDTO
            {
                FromUserRoleId = userRoleId,
                ToUserRoleIds = userRoleIds,
                InboxRequestTypeId = (byte)requestType,
                Placeholders = placeHolders
            };
            Log.Information($"| Strategy Name : {StrategyName} || Method ID : {MethodID} | Method Name : {MethodBase.GetCurrentMethod().Name} | Sending Notification Message.");
            Messenger.SendMessage(Command, requestDto);
            Log.Information("|{StrategyName}|{MethodID}| Notification Request DTO: {@RequestDTO}", StrategyName, MethodID, requestDto);
            Log.Information($"| Strategy Name : {StrategyName} || Method ID : {MethodID} | Method Name : {MethodBase.GetCurrentMethod().Name} | Ending Method ");
        }
        public Dictionary<string, string> SetPlaceHolders(ProductCodeSheetUploadHistory fileUploadHistory)
        {
            var placeholders = new Dictionary<string, string>();
            placeholders.Add("@FileName", fileUploadHistory.Name);
            placeholders.Add("@TotalRecordsCount", fileUploadHistory.TotalRecordsCount.ToString());
            placeholders.Add("@ProcessedRecordsCount", fileUploadHistory.ProcessedRecordsCount.ToString());
            return placeholders;
        }
        private void InsertProductCodeRecord(UnitOfWork uow, DataRow Row, short status, UploadConfigrationFileRequestDTO request, long fileUploadHistoryID, List<SheetAttributeMapping> propertyNameList, int userRoleId)
        {
            ProductCodeEntity productCodeEntity = new ProductCodeEntity();
            try
            {
                var ChapterCode = Row[0].ToString().Substring(0, 2);
                var ProductCodeChapter = uow.ProductCodeChapterRepository.Where(new { Code = ChapterCode }).FirstOrDefault();
                if (ProductCodeChapter != null)
                {
                    productCodeEntity.HSCode = Row[0].ToString();
                    productCodeEntity.HSCodeExt = Row[0].ToString() + "." + Row[1].ToString();
                    productCodeEntity.ProductCode = Row[1].ToString();
                    productCodeEntity.TradeTranTypeID = Convert.ToInt16(Row[3]);
                    productCodeEntity.ChapterCode = ChapterCode;
                    productCodeEntity.ProductCodeChapterID = (short)ProductCodeChapter.ID;
                    productCodeEntity.Description = Row[2].ToString();
                    productCodeEntity.ProductCodeSheetUploadHistoryID = fileUploadHistoryID;
                    productCodeEntity.EffectiveFromDt = Convert.ToDateTime(Row[4].ToString());
                    productCodeEntity.EffectiveThruDt = (String.IsNullOrEmpty(Row[5].ToString()) ||
                                                         String.IsNullOrWhiteSpace(Row[5].ToString())) ? DateTime.MaxValue : Convert.ToDateTime(Row[5].ToString()).AddHours(23).AddMinutes(59).AddSeconds(59);

                    productCodeEntity.CreatedOn = DateTime.Now;
                    productCodeEntity.UpdatedOn = DateTime.Now;
                    productCodeEntity.CreatedBy = userRoleId;
                    productCodeEntity.UpdatedBy = userRoleId;
                    // ACRHeader.CompletedOn = DateTime.Now;

                    var productCodeEntityId = uow.ProductCodeEntityRepository.Add(productCodeEntity);
                }
                else
                {
                    Log.Information("[{0}.{1}] Product Code Chapter Not Found {2} ", this.GetType().Name, MethodBase.GetCurrentMethod().Name, ProductCodeChapter);
                    throw new NullReferenceException(" Product Code Chapter Not Found ");

                }
            }
            catch (System.Exception ex)
            {
                Log.Error("[{0}.{1}] {2}-{3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex, ex.StackTrace);
                throw ex;
            }
        }
        private bool UpdateFileUploadHistory(UnitOfWork uow, long fileUploadHistoryID, short statusId, int userRoleId)
        {
            try
            {
                Log.Information("[{0}.{1}] Updating File History for file id: {2} as {3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, fileUploadHistoryID, statusId);

                var fileUploadHistory = uow.ProductCodeSheetUploadHistoryRepository.Get(fileUploadHistoryID.ToString());
                fileUploadHistory.ProductCodeSheetUploadStatusID = statusId;
                fileUploadHistory.UpdatedOn = DateTime.Now;
                fileUploadHistory.UpdatedBy = userRoleId;

                uow.ProductCodeSheetUploadHistoryRepository.Update(fileUploadHistory);

                Log.Information("[{0}.{1}] Updated File History for file id: {2} as {3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, fileUploadHistoryID, statusId);

                return true;
            }
            catch (System.Exception ex)
            {
                Log.Error("[{0}.{1}] {2}-{3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex, ex.StackTrace);
                throw ex;
            }
        }

        public DataTable GetDuplicateRecords(DataTable dt, DataTable duplicateTable, int errorColumnPosition)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            var duplicateCheckIndexList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType).Where(x => x.CheckDuplicate == true).Select(x => x.Index).ToList();
            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            var indexString = "";

            foreach (DataRow drow in dt.Rows)
            {
                foreach (int a in duplicateCheckIndexList)
                {
                    indexString += drow[a - 1].ToString();

                }
                if (hTable.Contains(indexString))
                    duplicateList.Add(drow);
                else
                    hTable.Add(indexString, string.Empty);
                indexString = "";
            }
            int rowIndex = 0;

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
            {
                rowIndex += 1;
                DataRow row = duplicateTable.NewRow();
                row.ItemArray = dRow.ItemArray;

                row[errorColumnPosition] = "Duplicate Row";
                row[errorColumnPosition + 1] = rowIndex + 1;
                duplicateTable.Rows.Add(row);
            }

            //Datatable which contains unique records will be return as output.
            return duplicateTable;

        }
        public DataRow AddToDisputedTable(DataTable dispuedTable, DataRow d, int hsCode, int productCode)
        {
            var ifNotExist = 0;

            DataRow row = dispuedTable.NewRow();
            if (dispuedTable.Rows.Count == 0)
            {
                row.ItemArray = d.ItemArray;
                return row;
            }
            foreach (DataRow i in dispuedTable.Rows)
            {
                if (i[productCode] == d[productCode].ToString() && i[hsCode] == d[hsCode].ToString())
                {
                    ifNotExist += 1;
                }

            }
            if (ifNotExist <= 0)
            {
                row.ItemArray = d.ItemArray;

                // dispuedTable.Rows.Add(row);
            }

            return row;
        }


        private IDictionary<string, string> IsFileColumnsCorrect(List<string> headerRow)
        {
            List<string> dbColumns = this.Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType).OrderBy(x => x.Index).Select(x => x.NameLong).ToList();//.Where(new { TradeTranTypeID = RequestDTO.TradeTranTypeID, AgencyID = RequestDTO.AgencyID, SheetType = RequestDTO.FileType, isActive = true }).OrderBy(x => x.Index).Select(x => x.NameLong).ToList();
            var res = new Dictionary<string, string>();
            var arraysAreEqual = Enumerable.SequenceEqual(dbColumns, headerRow);
            if (dbColumns.Count != headerRow.Count)
            {
                res.Add("column count mismatch", "");
                return res;
            }
            for (int i = 0; i < dbColumns.Count; i++)
            {
                if (dbColumns[i] != headerRow[i].Trim())
                {
                    res.Add(headerRow[i], dbColumns[i]);
                }
            }
            return res;

        }

        private string CheckIsMandatoryColumnsAvailable(DataTable dt)
        {
            var dbColumns = this.Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.FileType).Where(x => x.IsMandatory = true).ToList();


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DataRow row = dt.Rows[i];
                foreach (var allItems in dbColumns)
                {

                    if (row[allItems.Index - 1].ToString() == "")
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