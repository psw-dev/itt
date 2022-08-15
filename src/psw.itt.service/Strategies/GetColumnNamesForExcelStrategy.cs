using PSW.ITT.Service.DTO;
using psw.itt.service.Command;
using System;
using System.Collections.Generic;
using PSW.Lib.Logs;
using System.Security.Claims;
using System.Linq;
using psw.itt.data.Entities;

namespace psw.itt.service.Strategies
{
    public class GetColumnNamesForExcelStrategy : ApiStrategy<ColumnNamesForExcelRequestDTO, List<ColumnNamesForExcelResponseDTO>>
    {
        #region Constructors
        /// <summary>
        /// Strategy Constructor
        /// </summary>
        /// <param name="request">Takes CommandRequest</param>
        public GetColumnNamesForExcelStrategy(CommandRequest request) : base(request)
        {
            Reply = new CommandReply();
        }

        #endregion 

        #region Strategy Excecution 
        /// <summary>
        /// Strategy Execute Method
        /// </summary>
        /// <returns>Returns CommandReply</returns> 
        public override CommandReply Execute()
        {
            try
            {
                // STEP 1 : Validate Request DTO 


                Log.Information($"| Strategy Name : {StrategyName} | Method ID : {MethodID} | Started");

                Log.Information($"| Strategy Name : {StrategyName} | Method ID : {MethodID} | Reterieve Agency");

                var columnNames = this.Command.UnitOfWork.SheetAttributeMappingRepository
                .Where(new { isActive = true })
                .Select(x => new ColumnNamesForExcelResponseDTO { ColumnName = x.NameLong, Index = x.Index }).OrderBy(x => x.Index).ToList();
                if (columnNames.Count == 0)
                {
                    Log.Information($"| Strategy Name : {StrategyName} | Method ID : {MethodID} | No Record Found");
                    return NotFoundReply("No Record Found");
                }
                // STEP 3 : Prepare Response => Entity To DTO Mapping & Set Response DTO 
                Log.Information($"| Strategy Name : {StrategyName} | Method ID : {MethodID} | Mapping to responseDTO");
                ResponseDTO = columnNames;


                // STEP 4 : Send Command Reply
                Log.Information($"| Strategy Name : {StrategyName} | Method ID : {MethodID} | Column Name List reterived");

                return OKReply();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex, "| Class Name : {StrategyName} | Method ID : {MethodID} | An error occurred. {ErrorMessage} - {StackTrace}", StrategyName, MethodID, ex.Message, ex.StackTrace);
                //Command.UnitOfWork.Rollback();
                return InternalServerErrorReply(ex);
            }
            finally
            {
                Command.UnitOfWork.CloseConnection();
            }
        }

        #endregion

        #region Methods  

        #endregion 
    }
}
