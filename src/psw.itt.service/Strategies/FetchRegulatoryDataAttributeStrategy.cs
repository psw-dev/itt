using System;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;
using System.Linq;
using PSW.ITT.Service.ModelValidators;
using System.Collections.Generic;

namespace PSW.ITT.Service.Strategies
{
    public class FetchRegulatoryDataAttributeStrategy : ApiStrategy<FetchRegulatoryDataAttributeRequestDTO, List<FetchRegulatoryDataAttributeResponseDTO>>
    {
        private DateTime currentDateTime = DateTime.Now;
        #region Constructors
        public FetchRegulatoryDataAttributeStrategy(CommandRequest commandRequest) : base(commandRequest)
        {
            this.Validator = new FetchRegulatoryDataAttributeRequestDTOValidator();
        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);

                var agencyAttribute = Command.UnitOfWork.SheetAttributeMappingRepository.GetAgencyAttributeMapping(RequestDTO.AgencyID, RequestDTO.TradeTranTypeID,1);
                ResponseDTO = new List<FetchRegulatoryDataAttributeResponseDTO>();
                ResponseDTO = agencyAttribute.Select(item => Mapper.Map<FetchRegulatoryDataAttributeResponseDTO>(item)).ToList();


                // Prepare and return command reply
                return OKReply("Regulatory Data Attribute fetched Successfully");
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