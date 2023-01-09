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
    public class DownloadProductCodeExcelStrategy : ApiStrategy<Unspecified, DownloadProductCodeExcelResponseDTO>
    {
        #region Constructors
        public DownloadProductCodeExcelStrategy(CommandRequest commandRequest) : base(commandRequest)
        {

        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
                ResponseDTO = new DownloadProductCodeExcelResponseDTO();
                var productData = Command.UnitOfWork.ProductCodeEntityRepository.GetProductExcelData();
                ResponseDTO.Data = productData;
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

                var propertyNameList = this.Command.UnitOfWork.SheetAttributeMappingRepository.Where(new { isActive = true, SheetType = 4 }).OrderBy(x => x.Index).ToList();
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