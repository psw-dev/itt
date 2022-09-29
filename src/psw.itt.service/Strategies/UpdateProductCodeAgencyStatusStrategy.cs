using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using System.Collections.Generic;
using System;
using PSW.ITT.Data.Entities;

namespace PSW.ITT.Service.Strategies
{
    public class UpdateProductCodeAgencyStatusStrategy : ApiStrategy<UpdateProductCodeAgencyStatusRequestDTO, Unspecified>
    {

        DateTime currentTime = DateTime.Now;
        #region Constructors
        public UpdateProductCodeAgencyStatusStrategy(CommandRequest commandRequest) : base(commandRequest)
        {

        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
                var productAgencyLinkEntity = Command.UnitOfWork.ProductCodeAgencyLinkRepository.Get(RequestDTO.ID);
                if (RequestDTO.status)
                {
                    var productAgencyLinkEntityNew = new ProductCodeAgencyLink()
                    {
                        ProductCodeID = productAgencyLinkEntity.ProductCodeID,
                        AgencyID = productAgencyLinkEntity.AgencyID,
                        EffectiveFromDt = currentTime,
                        EffectiveThruDt = productAgencyLinkEntity.EffectiveThruDt,
                        CreatedBy = Command.LoggedInUserRoleID,
                        CreatedOn = currentTime,
                        UpdatedBy = Command.LoggedInUserRoleID,
                        UpdatedOn = currentTime,
                        IsActive = RequestDTO.status

                    };

                    productAgencyLinkEntity.EffectiveThruDt = currentTime;
                    productAgencyLinkEntity.UpdatedBy = Command.LoggedInUserRoleID;
                    productAgencyLinkEntity.UpdatedOn = currentTime;
                    Command.UnitOfWork.BeginTransaction();
                    Command.UnitOfWork.ProductCodeAgencyLinkRepository.Update(productAgencyLinkEntity);
                    Command.UnitOfWork.ProductCodeAgencyLinkRepository.Add(productAgencyLinkEntityNew);
                    Command.UnitOfWork.Commit();
                }
                else
                {
                    productAgencyLinkEntity.IsActive = RequestDTO.status;
                    productAgencyLinkEntity.UpdatedBy = Command.LoggedInUserRoleID;
                    productAgencyLinkEntity.UpdatedOn = currentTime;
                    Command.UnitOfWork.ProductCodeAgencyLinkRepository.Update(productAgencyLinkEntity);
                }



                return OKReply("Product Code Status Updated Successfully");
            }
            catch (ServiceException ex)
            {
                Log.Error("|{0}|{1}| Service exception caught: {2}", StrategyName, MethodID, ex.Message);
                Command.UnitOfWork.Rollback();
                return InternalServerErrorReply(ex);
            }
            catch (System.Exception ex)
            {
                Log.Error("|{0}|{1}| System exception caught: {2}", StrategyName, MethodID, ex.Message);
                Command.UnitOfWork.Rollback();
                return InternalServerErrorReply(ex);
            }
        }
        #endregion
    }
}