using System.Linq;
using System;
using System.Reflection;
using PSW.Lib.Logs;
using PSW.ITT.Service.Strategies;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Common;
using PSW.ITT.Common.Enums;

namespace PSW.ITT.Service.Strategies
{
    public class UpdateFileHistoryStatusStrategy : ApiStrategy<UploadFileHistoryStatusRequestDTO, UploadFileHistoryStatusResponseDTO>
    {
        #region Constructors

        /// <summary>
        /// Strategy Constructor
        /// </summary>
        /// <param name="request">Takes CommandRequest</param>
        public UpdateFileHistoryStatusStrategy(CommandRequest request) : base(request)
        {
            Reply = new CommandReply();
            // Validator = new UploadNationalRegisterRequestDTOValidator();
            Log.Information("| Strategy Name : {StrategyName} | Method ID : {MethodID} | Constructor Called", StrategyName, MethodID);
        }

        public override CommandReply Execute()
        {
            try
            {
                Log.Information("[{0}.{1}] Started", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                Log.Information("[{0}.{1}] Request DTO: {@RequestDTO}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, RequestDTO);

                int userRoleId = 0;
                var currentRole = Utility.GetCurrentUserRole(Command.UserClaims, RequestDTO.RoleCode);

                if (currentRole == null)
                {
                    Log.Information("[{0}.{1}] Invalid Role", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    return BadRequestReply("Invalid Role");
                }

                userRoleId = currentRole.UserRoleID;

                if (RequestDTO.StatusID != (int)ProductCodeSheetUploadStatusEnum.CANCELLED)
                {
                    Log.Warning("[{0}.{1}] Invalid Status provided:{2}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, RequestDTO.StatusID);
                    return BadRequestReply("Invalid Status provided");
                }

                // Getting current file which is in progress status
                var fileUploadHistory = Command.UnitOfWork.ProductCodeSheetUploadHistoryRepository.Where(new
                {
                    ID = RequestDTO.ID
                }).LastOrDefault();

                if (fileUploadHistory == null)
                {
                    Log.Warning("[{0}.{1}] No record found for File Upload History ID:{2}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, RequestDTO.ID);
                    return BadRequestReply("No record found");
                }

                if (fileUploadHistory.ProductCodeSheetUploadStatusID != (int)ProductCodeSheetUploadStatusEnum.IN_PROGRESS)
                {
                    Log.Warning("[{0}.{1}] Unable to cancel, No file is currently being processed.. File Upload History ID:{2}", this.GetType().Name, MethodBase.GetCurrentMethod().Name, RequestDTO.ID);
                    return BadRequestReply("Unable to cancel, No file is currently being processed.");
                }

                fileUploadHistory.ProductCodeSheetUploadStatusID = RequestDTO.StatusID;
                fileUploadHistory.UpdatedBy = userRoleId;
                fileUploadHistory.UpdatedOn = DateTime.Now;
                Command.UnitOfWork.ProductCodeSheetUploadHistoryRepository.Update(fileUploadHistory);

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
    }
}