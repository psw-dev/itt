using System;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using PSW.ITT.Data.Entities;
using System.Linq;
using PSW.ITT.Service.ModelValidators;
using PSW.ITT.Service.BusinessLogicLayer;
using System.Collections.Generic;

namespace PSW.ITT.Service.Strategies
{
    public class UploadSingleProductCodeStrategy : ApiStrategy<UploadSingleProductCodeRequestDTO, Unspecified>
    {
        IDictionary<string, short> tradeType = new Dictionary<string, short>(){
                { "I",1},
                { "E",2},
                { "T",3},
                { "A",4}
        };
        private DateTime currentDateTime = DateTime.Now;
        #region Constructors
        public UploadSingleProductCodeStrategy(CommandRequest commandRequest) : base(commandRequest)
        {
            this.Validator = new UploadSingleProductCodeRequestDTOValidator();
        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);

                try
                {
                    if (RequestDTO.EffectiveThruDt == null)
                    {
                        RequestDTO.EffectiveThruDt = new DateTime(9999, 12, 30);
                    }
                    DateTime endDate = (DateTime)RequestDTO.EffectiveThruDt;
                    RequestDTO.EffectiveFromDt = RequestDTO.EffectiveFromDt.AddDays(1);
                    RequestDTO.EffectiveThruDt = endDate.AddDays(1);
                    ProductCodeValidation PCValidator = new ProductCodeValidation(RequestDTO.HSCode, RequestDTO.ProductCode, RequestDTO.EffectiveFromDt, RequestDTO.EffectiveThruDt, Command, RequestDTO.TradeType);
                    PCValidator.validate();
                }
                catch (System.Exception ex)
                {
                    Log.Error("|{0}|{1}| {@ErrorMsg}", StrategyName, MethodID, ex.Message);
                    return BadRequestReply(ex.Message);
                }
                var ChapterCode = RequestDTO.HSCode.Substring(0, 2);
                var chapterID = Command.UnitOfWork.ProductCodeChapterRepository.Where(new
                {
                    Code = ChapterCode
                }).FirstOrDefault();

                var ProductCodeEntity = new ProductCodeEntity();
                ProductCodeEntity.HSCode = RequestDTO.HSCode;
                ProductCodeEntity.HSCodeExt = RequestDTO.HSCode + "." + RequestDTO.ProductCode;
                ProductCodeEntity.ProductCode = RequestDTO.ProductCode;
                ProductCodeEntity.ProductCodeChapterID = chapterID.ID;
                ProductCodeEntity.ChapterCode = ChapterCode;
                ProductCodeEntity.Description = RequestDTO.Description;
                ProductCodeEntity.ProductCodeSheetUploadHistoryID = null;
                ProductCodeEntity.EffectiveFromDt = RequestDTO.EffectiveFromDt;
                ProductCodeEntity.EffectiveThruDt = (DateTime)RequestDTO.EffectiveThruDt;
                ProductCodeEntity.CreatedBy = Command.LoggedInUserRoleID;
                ProductCodeEntity.CreatedOn = currentDateTime;
                ProductCodeEntity.UpdatedBy = Command.LoggedInUserRoleID;
                ProductCodeEntity.UpdatedOn = currentDateTime;
                ProductCodeEntity.TradeTranTypeID = RequestDTO.TradeType;
                Command.UnitOfWork.ProductCodeEntityRepository.Add(ProductCodeEntity);

                // Prepare and return command reply
                return OKReply("Product Code Uploaded Successfully");
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