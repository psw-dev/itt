using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using System.Collections.Generic;
using System;
using PSW.ITT.Data.Entities;

namespace PSW.ITT.Service.Strategies
{
    public class FetchActiveProductCodesListStrategy : ApiStrategy<FetchActiveProductCodesListRequestDTO, List<FetchActiveProductCodesListResponseDTO>>
    {
        IDictionary<long, string> tradeType = new Dictionary<long, string>(){
                {1, "Import"},
                {2, "Export"},
                {3, "Transit"},
                {4, "All"}
        };

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
                List<ProductCodeEntity> ActiveProductCodesList = new List<ProductCodeEntity>();

                if (RequestDTO.userRole == "OTO")
                {
                    ActiveProductCodesList = Command.UnitOfWork.ProductCodeEntityRepository.GetActiveAgencyProductCode((int)RequestDTO.agencyID, (short)RequestDTO.tradeTranTypeID);
                }
                else if (RequestDTO.userRole == "ITM")
                {
                    ActiveProductCodesList = Command.UnitOfWork.ProductCodeEntityRepository.GetActiveProductCode();
                }



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
                    if (tradeType.ContainsKey(item.TradeTranTypeID))
                    {
                        productCodeItem.TradeType = tradeType[item.TradeTranTypeID];
                    }
                    productCodeItem.EffectiveFromDt = item.EffectiveFromDt.ToString("dd-MM-yyyy");
                    if (DateTime.Compare(item.EffectiveThruDt.Date, new DateTime(9999, 12, 31).Date) == 0)
                    {
                        productCodeItem.EffectiveThruDt = null;
                    }
                    else
                    {
                        productCodeItem.EffectiveThruDt = item.EffectiveThruDt.ToString("dd-MM-yyyy");

                    }
                    if (RequestDTO.userRole == "OTO")
                    {
                        productCodeItem.IsActive = item.IsActive;
                        if ((bool)item.Regulated)
                        {
                            productCodeItem.Regulated = "R";
                        }
                    }

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