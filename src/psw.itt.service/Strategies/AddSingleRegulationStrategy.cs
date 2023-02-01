using System;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using PSW.ITT.Data.Entities;
using System.Text.Json;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PSW.ITT.Service.Strategies
{
    public class AddSingleRegulationStrategy : ApiStrategy<AddSingleRegulationRequestDTO, Unspecified>
    {
        private DateTime currentDateTime = DateTime.Now;
        #region Constructors
        public AddSingleRegulationStrategy(CommandRequest commandRequest) : base(commandRequest)
        {
            // this.Validator = new UploadSingleProductCodeRequestDTOValidator();
        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
                // var json = JsonConvert.DeserializeObject<JObject>(RequestDTO.Data);
                var json = JObject.Parse(RequestDTO.Data.ToString());

                var productcode = json.hsCode + "." + json.productCode;
                var duplicateCheckList = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.TradeTranTypeID, RequestDTO.AgencyID, 1).Where(x => x.CheckDuplicate == true).ToList();
                duplicateCheckList.RemoveAll(x => x.NameLong.Contains("HSCode"));
                duplicateCheckList.RemoveAll(x => x.NameLong.Contains("Product Code"));
                var factor = RequestDTO.Data.GetProperty(duplicateCheckList[0].NameShort).ToString();
                var factorObject = Command.SHRDUnitOfWork.ShrdCommonForLovRepository.GetLOV(duplicateCheckList.FirstOrDefault().TableName,duplicateCheckList.FirstOrDefault().ColumnName).Find(x=>x.Item2.ToLower()==RequestDTO.Data.GetProperty(duplicateCheckList[0].NameShort).ToString().ToLower());
            
                json.productCode = productcode;
                Command.UnitOfWork.BeginTransaction();
                var regulation = new LPCORegulation();
                regulation.AgencyID = RequestDTO.AgencyID;
                // regulation.RegulationJson = System.Text.Json.JsonSerializer.Serialize<dynamic>(json);
                regulation.RegulationJson = json.ToString(Formatting.None);
                regulation.CreatedOn = currentDateTime;
                regulation.UpdatedOn = currentDateTime;
                regulation.CreatedBy = Command.LoggedInUserRoleID;
                regulation.UpdatedBy = Command.LoggedInUserRoleID;
                regulation.HSCode = json.hsCode;
                regulation.HSCodeExt = json.productCode;
                regulation.ProductCodeAgencyLinkID = RequestDTO.ProductCodeAgencyLinkID;
                regulation.Factor = factorObject.Item2;
                regulation.FactorID=factorObject.Item1;
                // regulation.Factor = factor;
                regulation.EffectiveFromDt = currentDateTime;
                regulation.EffectiveThruDt = new DateTime(9999, 12, 30);
                regulation.TradeTranTypeID = RequestDTO.TradeTranTypeID;
                var LPCOid = Command.UnitOfWork.LPCORegulationRepository.Add(regulation);

                // var regulationAgencyLink = new ProductRegulationRequirement();
                // regulationAgencyLink.ProductCodeAgencyLinkID = RequestDTO.ProductCodeAgencyLinkID;
                // regulationAgencyLink.LPCOFeeStructureID = 3; //TODO: remove hardcoded value
                // regulationAgencyLink.LPCORegulationID = LPCOid;
                // regulationAgencyLink.EffectiveFromDt = currentDateTime;
                // regulationAgencyLink.EffectiveThruDt = new DateTime(9999, 12, 30);
                // regulationAgencyLink.CreatedOn = currentDateTime;
                // regulationAgencyLink.UpdatedOn = currentDateTime;
                // regulationAgencyLink.CreatedBy = Command.LoggedInUserRoleID;
                // regulationAgencyLink.UpdatedBy = Command.LoggedInUserRoleID;
                // regulationAgencyLink.TradeTranTypeID = RequestDTO.TradeTranTypeID;
                // Command.UnitOfWork.ProductRegulationRequirementRepository.Add(regulationAgencyLink);
                Command.UnitOfWork.Commit();
                // Prepare and return command reply
                return OKReply("Regulation Uploaded Successfully");
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