using System;
using psw.itt.service.Command;
using PSW.ITT.Service.DTO;
using psw.itt.service.Exception;
using PSW.Lib.Logs;
using System.Linq;
using PSW.SD.Service.ModelValidators;

namespace psw.itt.service.Strategies
{
    public class EditProductCode : ApiStrategy<EditProductCodeRequestDTO, Unspecified>
    {
        #region Constructors
        public EditProductCode(CommandRequest commandRequest) : base(commandRequest)
        {
            this.Validator = new EditProductCodeRequestDTOValidator();
        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
                var ProductCodeEntity = Command.UnitOfWork.ProductCodeEntityRepository.Where(new
                {
                    HSCode = RequestDTO.HSCode,
                    ProductCode = RequestDTO.ProductCode
                }).FirstOrDefault();

                ProductCodeEntity.EffectiveFromDt = RequestDTO.EffectiveFromDt;
                ProductCodeEntity.EffectiveThruDt = RequestDTO.EffectiveThruDt;
                ProductCodeEntity.UpdatedBy = Command.LoggedInUserRoleID;
                ProductCodeEntity.UpdatedOn = DateTime.Now;

                Command.UnitOfWork.ProductCodeEntityRepository.Update(ProductCodeEntity);


                // Prepare and return command reply
                return OKReply("Product Code Update Sucessfully");
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