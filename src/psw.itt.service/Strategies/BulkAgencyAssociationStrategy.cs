using System;
using PSW.ITT.Data.Entities;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using System.Linq;

namespace PSW.ITT.Service.Strategies
{
    public class BulkAgencyAssociationStrategy : ApiStrategy<BulkAgencyAssociationRequestDTO, Unspecified>
    {
        #region Constructors
        public BulkAgencyAssociationStrategy(CommandRequest commandRequest) : base(commandRequest)
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
                return OKReply("Bulk Agency Association Updated Successfully");
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
                var agencyAssociationList = Command.UnitOfWork.ProductCodeAgencyLinkRepository.Get();

                foreach (var item in RequestDTO.ProductCodes)
                {
                    var productEntity = Command.UnitOfWork.ProductCodeEntityRepository.Get(item);
                    if (agencyAssociationList.Any(x => x.AgencyID == RequestDTO.AgencyID && x.ProductCodeID == item && x.EffectiveThruDt >= DateTime.Now))
                    {
                        var productCodeAgencyLinkEntity = new ProductCodeAgencyLink()
                        {
                            ProductCodeID = item,
                            AgencyID = RequestDTO.AgencyID,
                            EffectiveFromDt = productEntity.EffectiveFromDt,
                            EffectiveThruDt = productEntity.EffectiveThruDt,
                            CreatedBy = Command.LoggedInUserRoleID,
                            UpdatedBy = Command.LoggedInUserRoleID,
                            CreatedOn = DateTime.Now,
                            UpdatedOn = DateTime.Now
                        };
                        Command.UnitOfWork.ProductCodeAgencyLinkRepository.Add(productCodeAgencyLinkEntity);
                    }
                }

            }

            void REMOVEOGA()
            {
                foreach (var item in RequestDTO.ProductCodes)
                {
                    var productAgencyLinkEntity = Command.UnitOfWork.ProductCodeAgencyLinkRepository.Where(
                                      new
                                      {
                                          ProductCodeID = item,
                                          AgencyID = RequestDTO.AgencyID
                                      }
                                  ).FirstOrDefault();
                    productAgencyLinkEntity.UpdatedBy = Command.LoggedInUserRoleID;
                    productAgencyLinkEntity.UpdatedOn = DateTime.Now;
                    productAgencyLinkEntity.EffectiveThruDt = DateTime.Now;
                    Command.UnitOfWork.ProductCodeAgencyLinkRepository.Update(productAgencyLinkEntity);
                }

            }

        }
        #endregion
    }
}