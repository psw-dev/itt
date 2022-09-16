using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using System.Linq;
using System.Collections.Generic;

namespace PSW.ITT.Service.Strategies
{
    public class FetchActiveProductCodesListStrategy : ApiStrategy<Unspecified, List<FetchActiveProductCodesListResponseDTO>>
    {

        #region Constructors
        public FetchActiveProductCodesListStrategy(CommandRequest commandRequest) : base(commandRequest)
        {

        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
                var ActiveProductCodesList = Command.UnitOfWork.ProductCodeEntityRepository.GetActiveProductCode();

                ResponseDTO = new List<FetchActiveProductCodesListResponseDTO>();
                foreach (var item in ActiveProductCodesList)
                {
                    var productCodeItem = new FetchActiveProductCodesListResponseDTO();
                    productCodeItem.SerialID = item.SerialID;
                    productCodeItem.ID = item.ID;
                    productCodeItem.HSCode = item.HSCode;//GetFileName(listFileDetails, item.AttachedFileID);
                    productCodeItem.HSCodeExt = item.HSCodeExt;
                    productCodeItem.ProductCode = item.ProductCode;
                    productCodeItem.ProductCodeChapterID = (short)item.ProductCodeChapterID;
                    productCodeItem.ChapterCode = item.ChapterCode;
                    productCodeItem.Description = item.Description;
                    productCodeItem.EffectiveFromDt = item.EffectiveFromDt.ToString("dd-MMM-yyyy");
                    productCodeItem.EffectiveThruDt = item.EffectiveThruDt.ToString("dd-MMM-yyyy");

                    ResponseDTO.Add(productCodeItem);
                }

                return OKReply("Active Product Code List Fetched Successfully");
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
    }
}