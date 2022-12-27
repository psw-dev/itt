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
                        EffectiveFromDt = productAgencyLinkEntity.EffectiveFromDt,
                        EffectiveThruDt = productAgencyLinkEntity.EffectiveThruDt,
                        RegulationEffectiveFromDt = currentTime,
                        RegulationEffectiveThruDt = productAgencyLinkEntity.EffectiveThruDt,
                        CreatedBy = Command.LoggedInUserRoleID,
                        CreatedOn = currentTime,
                        UpdatedBy = Command.LoggedInUserRoleID,
                        UpdatedOn = currentTime,
                        IsActive = RequestDTO.status,
                        SoftDelete = false

                    };

                    productAgencyLinkEntity.UpdatedBy = Command.LoggedInUserRoleID;
                    productAgencyLinkEntity.UpdatedOn = currentTime;
                    productAgencyLinkEntity.SoftDelete = true;
                    Command.UnitOfWork.BeginTransaction();
                    Command.UnitOfWork.ProductCodeAgencyLinkRepository.Update(productAgencyLinkEntity);
                    var productAgencyID = Command.UnitOfWork.ProductCodeAgencyLinkRepository.Add(productAgencyLinkEntityNew);
                    var regulationList = Command.UnitOfWork.LPCORegulationRepository.GetRegulationByProductAgencyLinkID(RequestDTO.ID);
                    foreach (var regulation in regulationList)
                    {
                        regulation.ProductCodeAgencyLinkID = productAgencyID;
                        regulation.UpdatedBy = Command.LoggedInUserRoleID;
                        regulation.UpdatedOn = currentTime;
                        Command.UnitOfWork.LPCORegulationRepository.Update(regulation);
                    }

                    Command.UnitOfWork.Commit();
                }
                else
                {
                    productAgencyLinkEntity.IsActive = RequestDTO.status;
                    productAgencyLinkEntity.UpdatedBy = Command.LoggedInUserRoleID;
                    productAgencyLinkEntity.UpdatedOn = currentTime;
                    productAgencyLinkEntity.RegulationEffectiveThruDt = currentTime;
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