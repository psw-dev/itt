using System;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using PSW.ITT.Data.Entities;
using System.Linq;
using PSW.ITT.Service.ModelValidators;
using PSW.ITT.Service.BusinessLogicLayer;

namespace PSW.ITT.Service.Strategies
{
    public class UploadSingleProductCode : ApiStrategy<UploadSingleProductCodeRequestDTO, Unspecified>
    {
        private DateTime currentDateTime = DateTime.Now;
        #region Constructors
        public UploadSingleProductCode(CommandRequest commandRequest) : base(commandRequest)
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
                    ProductCodeValidation PCValidator = new ProductCodeValidation(RequestDTO.HSCode, RequestDTO.ProductCode, RequestDTO.EffectiveFromDt, RequestDTO.EffectiveThruDt, Command);
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
                ProductCodeEntity.EffectiveThruDt = RequestDTO.EffectiveThruDt;
                ProductCodeEntity.CreatedBy = Command.LoggedInUserRoleID;
                ProductCodeEntity.CreatedOn = currentDateTime;
                ProductCodeEntity.UpdatedBy = Command.LoggedInUserRoleID;
                ProductCodeEntity.UpdatedOn = currentDateTime;

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