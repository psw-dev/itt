using System.Linq;
using System;
using System.Reflection;
using PSW.Lib.Logs;
using System.Security.Claims;
using psw.itt.service.Strategies;
using psw.itt.service.Command;
using PSW.OGA.Service.DTO;
using PSW.itt.Common.Enums;

namespace psw.itt.service.Strategies
{
    public class GetUploadFileProgressStrategy : ApiStrategy<UploadProgressRequestDTO, UploadProgressResponseDTO>
    {
        #region Constructors

        /// <summary>
        /// Strategy Constructor
        /// </summary>
        /// <param name="request">Takes CommandRequest</param>
        public GetUploadFileProgressStrategy(CommandRequest request) : base(request)
        {
            Reply = new CommandReply();
            // Validator = new UploadNationalRegisterRequestDTOValidator();
            Log.Information("| Strategy Name : {StrategyName} | Method ID : {MethodID} | Constructor Called", StrategyName, MethodID);
        }

        #endregion

        #region Strategy Excecution

        /// <summary>
        /// Strategy Execute Method
        /// </summary>
        /// <returns>Returns CommandReply</returns>
        private int UserId
        {
            get { return Convert.ToInt32(GetUserID()); }
        }

        public override CommandReply Execute()
        {
            try
            {
                Log.Information("[{0}.{1}] Started", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                Log.Information("[{0}.{1}]  Request DTO: {@RequestDTO}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, RequestDTO);

                ResponseDTO = new UploadProgressResponseDTO();

                var fileUploadHistory = Command.UnitOfWork.ProductCodeSheetUploadHistoryRepository.All().LastOrDefault();


                if (fileUploadHistory != null)
                {
                    switch (fileUploadHistory.ProductCodeSheetUploadStatusID)
                    {
                        case (int)ProductCodeSheetUploadStatusEnum.IN_PROGRESS:
                            ResponseDTO.DisputedRecordsData = "";
                            ResponseDTO.TotalRecordsCount = (fileUploadHistory.TotalRecordsCount - fileUploadHistory.DuplicateRecordsCount);
                            ResponseDTO.DisputedRecordsCount = fileUploadHistory.DisputedRecordsCount;
                            ResponseDTO.ProcessedRecordsCount = fileUploadHistory.ProcessedRecordsCount;
                            ResponseDTO.StatusId = (int)ProductCodeSheetUploadStatusEnum.IN_PROGRESS;
                            break;
                        case (int)ProductCodeSheetUploadStatusEnum.FAILED:
                            // ResponseDTO.DisputedRecordsData = fileUploadHistory.DisputedRecordsData;
                            ResponseDTO.TotalRecordsCount = (fileUploadHistory.TotalRecordsCount - fileUploadHistory.DuplicateRecordsCount);
                            ResponseDTO.DisputedRecordsCount = fileUploadHistory.DisputedRecordsCount;
                            ResponseDTO.ProcessedRecordsCount = fileUploadHistory.ProcessedRecordsCount;
                            ResponseDTO.StatusId = (int)ProductCodeSheetUploadStatusEnum.FAILED;
                            break;
                        case (int)ProductCodeSheetUploadStatusEnum.PROCESSED:
                            ResponseDTO.DisputedRecordsData = "";
                            ResponseDTO.TotalRecordsCount = (fileUploadHistory.TotalRecordsCount - fileUploadHistory.DuplicateRecordsCount);
                            ResponseDTO.DisputedRecordsCount = fileUploadHistory.DisputedRecordsCount;
                            ResponseDTO.ProcessedRecordsCount = fileUploadHistory.ProcessedRecordsCount;
                            ResponseDTO.StatusId = (int)ProductCodeSheetUploadStatusEnum.PROCESSED;
                            break;
                    }
                }

                return OKReply();
            }
            catch (System.Exception ex)
            {
                Log.Error("[{0}.{1}] {2}-{3}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex, ex.StackTrace);
                return InternalServerErrorReply(ex);
            }
            finally
            {
                Command.UnitOfWork.CloseConnection();
            }
        }

        #endregion

        private string GetUserID()
        {
            try
            {
                Claim userIdClaim = this.Command.UserClaims.Where(x => x.Type == "subscriptionId").ToList().ElementAt(0);
                Console.WriteLine($"Claim: {userIdClaim.Value}");
                return "0";
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

   
    }
}