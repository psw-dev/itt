using System;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using System.Linq;
using PSW.ITT.Service.ModelValidators;
using System.Collections.Generic;
using PSW.ITT.Data.Entities;

namespace PSW.ITT.Service.Strategies
{
    public class FetchLOVDataStrategy : ApiStrategy<FetchLOVDataRequestDTO, List<FetchLOVDataResponseDTO>>
    {
        private DateTime currentDateTime = DateTime.Now;
        #region Constructors
        public FetchLOVDataStrategy(CommandRequest commandRequest) : base(commandRequest)
        {
            this.Validator = new FetchLOVDataRequestDTOValidator();
        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);

                var lovDataFromDBList = new List<FetchLOVDataResponseDTO>();
                var lovItem = new FetchLOVDataResponseDTO();

                var hsCode = Command.UnitOfWork.ProductCodeEntityRepository.GetActiveAgencyProductCodeLOV(RequestDTO.AgencyID, RequestDTO.TradeTranTypeID, "ProductCode", "HSCode");
                lovItem.LOVItems = hsCode;
                lovItem.LOVTableName = "HSCode";
                lovDataFromDBList.Add(lovItem);

                var productCode = Command.UnitOfWork.ProductCodeEntityRepository.GetActiveAgencyProductCodeLOV(RequestDTO.AgencyID, RequestDTO.TradeTranTypeID, "ProductCode", "ProductCode");
                lovItem = new FetchLOVDataResponseDTO();
                lovItem.LOVItems = productCode;
                lovItem.LOVTableName = "ProductCode";

                lovDataFromDBList.Add(lovItem);
                var documentList = Command.UnitOfWork.ProductCodeEntityRepository.GetDocumentLOV(RequestDTO.AgencyID, "DocumentType", "Name",RequestDTO.TradeTranTypeID);
                lovItem = new FetchLOVDataResponseDTO();
                lovItem.LOVItems = documentList;
                lovItem.LOVTableName = "ITTAgencyDocuments";

                lovDataFromDBList.Add(lovItem);

                var calculationBasis = Command.UnitOfWork.CalculationBasisRepository.Get().Select(x=> new LOVItem { ItemKey = x.ID.ToString(), ItemValue= x.Description}).ToList();
                lovItem = new FetchLOVDataResponseDTO();
                lovItem.LOVItems = calculationBasis;
                lovItem.LOVTableName = "CalculationBasis";

                lovDataFromDBList.Add(lovItem);

                var calculationSource = Command.UnitOfWork.CalculationSourceRepository.Get().Select(x=> new LOVItem{ ItemKey = x.ID.ToString(), ItemValue= x.Description}).ToList();
                lovItem = new FetchLOVDataResponseDTO();
                lovItem.LOVItems = calculationSource;
                lovItem.LOVTableName = "CalculationSource";
                
                lovDataFromDBList.Add(lovItem);

                ResponseDTO = lovDataFromDBList;

                // Prepare and return command reply
                return OKReply("ITT LOV Data fetched Successfully");
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
    }
}