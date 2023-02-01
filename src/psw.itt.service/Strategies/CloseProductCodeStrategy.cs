using System;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using System.Linq;
using PSW.ITT.Service.ModelValidators;
using psw.itt.service.Helpers;
using System.Collections.Generic;
using System.Reflection;
using PSW.itt.Common.Enums;
using PSW.ITT.Data.Entities;

namespace PSW.ITT.Service.Strategies
{
    public class CloseProductCodeStrategy : ApiStrategy<CloseProductCodeRequestDTO, Unspecified>
    {
        private DateTime currentDateTime = DateTime.Now;
        private bool sendInboxMsg = false;
        #region Constructors
        public CloseProductCodeStrategy(CommandRequest commandRequest) : base(commandRequest)
        {
            this.Validator = new CloseProductCodeRequestDTOValidator();
        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
                var ProductCodeEntity = Command.UnitOfWork.ProductCodeEntityRepository.Get(RequestDTO.ID);
                if (ProductCodeEntity.EffectiveThruDt <= currentDateTime)
                {
                    return BadRequestReply("Product Code already deactivated");
                }
                Command.UnitOfWork.BeginTransaction();
                ProductCodeEntity.EffectiveThruDt = currentDateTime.AddDays(1).Date.AddSeconds(-1);
                ProductCodeEntity.UpdatedBy = Command.LoggedInUserRoleID;
                ProductCodeEntity.UpdatedOn = currentDateTime;
                // This will set time at max of same day
                // as Product code will be closed at the end of day 
                Command.UnitOfWork.ProductCodeEntityRepository.Update(ProductCodeEntity);

                var agencyLinkList = Command.UnitOfWork.ProductCodeAgencyLinkRepository
                .Where(new { ProductCodeID = ProductCodeEntity.ID });
                foreach (var agencyLink in agencyLinkList)
                {
                    sendInboxMsg = true;
                    agencyLink.UpdatedBy = Command.LoggedInUserRoleID;
                    agencyLink.UpdatedOn = currentDateTime;
                    agencyLink.EffectiveThruDt = currentDateTime.AddDays(1).Date.AddSeconds(-1);
                    Command.UnitOfWork.ProductCodeAgencyLinkRepository.Update(agencyLink);
                    var agencyLinkRegulation = Command.UnitOfWork.LPCORegulationRepository.GetRegulationByProductAgencyLinkID(agencyLink.ID);
                    foreach (var regulation in agencyLinkRegulation)
                    {
                        regulation.UpdatedBy = Command.LoggedInUserRoleID;
                        regulation.UpdatedOn = currentDateTime;
                        regulation.EffectiveThruDt = currentDateTime.AddDays(1).Date.AddSeconds(-1);
                        Command.UnitOfWork.LPCORegulationRepository.Update(regulation);
                    }
                }

                Command.UnitOfWork.Commit();
                if (sendInboxMsg)
                {
                    SendMessage(ProductCodeEntity, InboxRequestType.REGULATION_DEACTIVATE);
                }
                // Prepare and return command reply
                return OKReply("Product Code Closed Successfully");
            }
            catch (ServiceException ex)
            {
                Log.Error("|{0}|{1}| Service exception caught: {2}", StrategyName, MethodID, ex.Message);
                Command.UnitOfWork.Rollback();
                return InternalServerErrorReply(ex);
            }
            catch (System.Exception ex)
            {
                Command.UnitOfWork.Rollback();
                Log.Error("|{0}|{1}| System exception caught: {2}", StrategyName, MethodID, ex.Message);
                return InternalServerErrorReply(ex);
            }
        }
        #endregion



        private void SendMessage(ProductCodeEntity productCode, InboxRequestType requestType)
        {
            Log.Information($"| Strategy Name : {StrategyName} || Method ID : {MethodID} | Method Name : {MethodBase.GetCurrentMethod().Name} | Starting Method ");
            List<int> userRoleIds = new List<int>();
            userRoleIds = Command.UnitOfWork.ProductCodeAgencyLinkRepository.GetAllOTORoleIDAssociatedWithProductCode(productCode.ID);
            Dictionary<string, string> placeHolders = SetPlaceHolders(productCode);
            var requestDto = new SendInboxMessageRequestDTO
            {
                FromUserRoleId = 0,
                ToUserRoleIds = userRoleIds,
                InboxRequestTypeId = (byte)requestType,
                Placeholders = placeHolders
            };
            Log.Information($"| Strategy Name : {StrategyName} || Method ID : {MethodID} | Method Name : {MethodBase.GetCurrentMethod().Name} | Sending Notification Message.");
            Messenger.SendMessage(Command, requestDto);
            Log.Information("|{StrategyName}|{MethodID}| Notification Request DTO: {@RequestDTO}", StrategyName, MethodID, requestDto);
            Log.Information($"| Strategy Name : {StrategyName} || Method ID : {MethodID} | Method Name : {MethodBase.GetCurrentMethod().Name} | Ending Method ");
        }
        public Dictionary<string, string> SetPlaceHolders(ProductCodeEntity productCode)
        {
            var placeholders = new Dictionary<string, string>();
            placeholders.Add("@productCode", productCode.HSCodeExt);
            placeholders.Add("@productDescription", productCode.Description);
            return placeholders;
        }
    }
}