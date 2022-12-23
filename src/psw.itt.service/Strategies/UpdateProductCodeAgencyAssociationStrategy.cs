using System;
using PSW.ITT.Data.Entities;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using System.Linq;

namespace PSW.ITT.Service.Strategies
{
    public class UpdateProductCodeAgencyAssociationStrategy : ApiStrategy<UpdateProductCodeAgencyAssociationRequestDTO, Unspecified>
    {
        #region Constructors
        public UpdateProductCodeAgencyAssociationStrategy(CommandRequest commandRequest) : base(commandRequest)
        {

        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);

                if (RequestDTO.isAdd)
                {
                    ADDOGA();
                }
                else
                {
                    REMOVEOGA();
                }


                ResponseDTO = new Unspecified();
                // Prepare and return command reply
                return OKReply("Agency Association Updated Successfully");
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

            void ADDOGA()
            {
                var productEntity = Command.UnitOfWork.ProductCodeEntityRepository.Get(RequestDTO.ProductCodeID);
                var productCodeAgencyLinkEntity = new ProductCodeAgencyLink()
                {
                    ProductCodeID = RequestDTO.ProductCodeID,
                    AgencyID = RequestDTO.AgencyID,
                    EffectiveFromDt = DateTime.Now,
                    EffectiveThruDt = productEntity.EffectiveThruDt,
                    RegulationEffectiveFromDt = DateTime.Now,
                    RegulationEffectiveThruDt = productEntity.EffectiveThruDt,
                    CreatedBy = Command.LoggedInUserRoleID,
                    UpdatedBy = Command.LoggedInUserRoleID,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    IsActive = true,
                    SoftDelete = false
                };
                Command.UnitOfWork.ProductCodeAgencyLinkRepository.Add(productCodeAgencyLinkEntity);
            }

            void REMOVEOGA()
            {
                var productAgencyLinkEntity = Command.UnitOfWork.ProductCodeAgencyLinkRepository.Where(
                    new
                    {
                        ProductCodeID = RequestDTO.ProductCodeID,
                        AgencyID = RequestDTO.AgencyID
                    }
                ).FirstOrDefault();
                productAgencyLinkEntity.UpdatedBy = Command.LoggedInUserRoleID;
                productAgencyLinkEntity.UpdatedOn = DateTime.Now;
                productAgencyLinkEntity.EffectiveThruDt = DateTime.Now;
                Command.UnitOfWork.ProductCodeAgencyLinkRepository.Update(productAgencyLinkEntity);
            }

        }
        #endregion
    }
}