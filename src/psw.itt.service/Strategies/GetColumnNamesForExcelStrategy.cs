using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Command;
using System;
using System.Collections.Generic;
using PSW.Lib.Logs;
using System.Security.Claims;
using System.Linq;
using PSW.ITT.Data.Entities;
using PSW.ITT.Common.Enums;

namespace PSW.ITT.Service.Strategies
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

                var columnNames = new List<ColumnNamesForExcelResponseDTO>();

                if (RequestDTO.SheetType == (short)FileTypeEnum.ADD_PRODUCTCODE_TEMPLATE)
                {
                    // var FileType=(short)FileTypeEnum.ADD_PRODUCTCODE_TEMPLATE;
                    columnNames = this.Command.UnitOfWork.SheetAttributeMappingRepository
                    .Where(new { isActive = true }).Where(x => x.SheetType == RequestDTO.SheetType)
                    .Select(x => new ColumnNamesForExcelResponseDTO { ColumnName = x.NameLong, Index = x.Index }).OrderBy(x => x.Index).ToList();
                }
                else if(RequestDTO.SheetType == (short)FileTypeEnum.ADD_REGULATIONS_TEMPLATE || RequestDTO.SheetType == (short)FileTypeEnum.UPDATE_REGULATIONS_TEMPLATE || RequestDTO.SheetType == (short)FileTypeEnum.INACTIVATE_REGULATIONS_TEMPLATE)
                {
                    columnNames = this.Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, RequestDTO.ActionID ==  (short)FileTypeEnum.UPDATE_REGULATIONS_TEMPLATE ? (short)FileTypeEnum.ADD_REGULATIONS_TEMPLATE : RequestDTO.ActionID).Select(x => new ColumnNamesForExcelResponseDTO { ColumnName = x.NameLong, Index = x.Index }).ToList();
                }
                else if(RequestDTO.SheetType == (short)FileTypeEnum.VALIDATE_PRODUCTCODE_TEMPLETE)
                {
                    // var FileType=(short)FileTypeEnum.VALIDATE_PRODUCTCODE_TEMPLETE;
                    columnNames = this.Command.UnitOfWork.SheetAttributeMappingRepository
                    .Where(new { isActive = true }).Where(x => x.SheetType == RequestDTO.SheetType)
                    .Select(x => new ColumnNamesForExcelResponseDTO { ColumnName = x.NameLong, Index = x.Index }).OrderBy(x => x.Index).ToList();
                }

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
