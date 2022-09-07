using System;
using System.Collections.Generic;
using System.Linq;
using PSW.ITT.Service.Command;
using PSW.ITT.Service.DTO;
using PSW.ITT.Service.Exception;
using PSW.Lib.Logs;

namespace PSW.ITT.Service.Strategies
{
    public class GetProductCodeListWithAgenciesStrategy : ApiStrategy<Unspecified, List<GetProductCodeListWithAgenciesResponseDTO>>
    {
        #region Constructors
        public GetProductCodeListWithAgenciesStrategy(CommandRequest commandRequest) : base(commandRequest)
        {

        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);



                var ProductCodesWithOGAs = Command.UnitOfWork.ProductCodeAgencyLinkRepository.GetProductCodeIDWithOGA();
                ResponseDTO = new List<GetProductCodeListWithAgenciesResponseDTO>();
                ResponseDTO = ProductCodesWithOGAs.Select(item => Mapper.Map<GetProductCodeListWithAgenciesResponseDTO>(item)).ToList();


                // Prepare and return command reply
                return OKReply("Product Code with OGAs Fetched Successfully");
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