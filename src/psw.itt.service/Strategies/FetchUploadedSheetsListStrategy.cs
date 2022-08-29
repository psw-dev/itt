using System.Linq;
using System.Collections.Generic;
using System;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Data.Entities;
using PSW.Lib.Logs;
using System.Security.Claims;
using PSW.ITT.Common.Enums;
using System.Threading;
using PSW.ITT.Service.Helpers;
using psw.oga.service.Helpers;

namespace PSW.ITT.Service.Strategies
{
    public class FetchUploadedSheetsListStrategy : ApiStrategy<UploadFileHistoryRequestDTO, List<UploadFileHistoryResponseDTO>>
    {
        #region Constructors

        /// <summary>
        /// Strategy Constructor
        /// </summary>
        /// <param name="request">Takes CommandRequest</param>
        public FetchUploadedSheetsListStrategy(CommandRequest request) : base(request)
        {
            Reply = new CommandReply();
            // Validator = new UploadNationalRegisterRequestDTOValidator();
            Log.Information("| Strategy Name : {StrategyName} | Method ID : {MethodID} | Constructor Called", StrategyName, MethodID);
        }

        #endregion

        #region Strategy Excecution

        public override CommandReply Execute()
        {
            try
            {
                Log.Information($"| Strategy Name : {StrategyName} | Method ID : {MethodID} | Started");
                Log.Information($"| Strategy Name : {StrategyName} | Method ID : {MethodID} | Request DTO: {RequestDTO}");

                ResponseDTO = new List<UploadFileHistoryResponseDTO>();

                var listFileUploadHistory = Command.UnitOfWork.ProductCodeSheetUploadHistoryRepository.All().OrderByDescending(x => x.UpdatedOn).ToList();

                if (listFileUploadHistory == null || listFileUploadHistory.Count == 0)
                {
                    return BadRequestReply("No file uploaded");
                }

                // Get File Ids from FSS
                // List<string> fileIds = new List<string>();
                // fileIds.AddRange(listFileUploadHistory.Select(x => Command.CryptoAlgorithm.Encrypt(x.AttachedFileID.ToString())));

                // var listFileDetails = FileHelper.GetFilesDetails(Command, fileIds);

                var last = listFileUploadHistory.First();
                foreach (var item in listFileUploadHistory)
                {
                    var fileUploadItem = new UploadFileHistoryResponseDTO();

                    fileUploadItem.ID = item.ID;
                    // fileUploadItem.OwnerDocumentTypeCode = item.OwnerDocumentTypeCode;
                    // fileUploadItem.OwnerDocumentTypeName = GetDocumentTypeCode(item.OwnerDocumentTypeCode).Name.ToString();
                    // fileUploadItem.AgencyID = item.AgencyID;
                    fileUploadItem.AttachedFileID = Command.CryptoAlgorithm.Encrypt(item.AttachedFileID.ToString());
                    fileUploadItem.AttachedFileName = item.Name;//GetFileName(listFileDetails, item.AttachedFileID);
                    fileUploadItem.TotalRecordsCount = item.TotalRecordsCount;
                    fileUploadItem.DisputedRecordsCount = item.DisputedRecordsCount;
                    fileUploadItem.DuplicateRecordsCount = item.DuplicateRecordsCount;
                    fileUploadItem.ProcessedRecordsCount = item.ProcessedRecordsCount;
                    // fileUploadItem.DisputedRecordsData = item.DisputedRecordsData;
                    fileUploadItem.StatusId = item.ProductCodeSheetUploadStatusID;
                    fileUploadItem.StatusName = EnumHelper.GetEnumDescription(((ProductCodeSheetUploadStatusEnum)item.ProductCodeSheetUploadStatusID));
                    fileUploadItem.CreatedOn = item.CreatedOn.ToString("dd-MMM-yyyy");
                    fileUploadItem.UpdatedOn = item.UpdatedOn.ToString("dd-MMM-yyyy");
                    fileUploadItem.IsLast = (item.ID == last.ID) ? true : false;
                    fileUploadItem.UploadedBy = UMSHelper.GetUserInformation(Command, (int)item.CreatedBy)?.PersonName ?? "";

                    ResponseDTO.Add(fileUploadItem);
                }

                return OKReply();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "| Class Name : {StrategyName} | Method ID : {MethodID} | An error occurred. {ErrorMessage} - {StackTrace}", StrategyName, MethodID, ex.Message, ex.StackTrace);
                Command.UnitOfWork.Rollback();
                return InternalServerErrorReply(ex);
            }
            finally
            {
                Command.UnitOfWork.CloseConnection();
            }
        }

        #endregion

        // private DocumentType GetDocumentTypeCode(string documentTypeCode)
        // {
        //     return Command.UnitOfWork.DocumentTypeRepository.Where(new
        //     {
        //         code = documentTypeCode
        //     }).FirstOrDefault();
        // }

        private string GetStatus(int statusId)
        {
            string status = Enum.GetName(typeof(ProductCodeSheetUploadStatusEnum), statusId);
            return StringHelper.CamelCase(status.ToString());
        }

        private string GetFileName(GetFilesDetailResponseDTO listFileDetails, long fileId)
        {
            var fileDetails = listFileDetails.Files.FirstOrDefault(x => x.Id == fileId);
            return fileDetails?.FileNameDisplay ?? "";
        }
    }
}