using System;
using psw.itt.service.Command;
using PSW.ITT.Service.DTO;
using psw.itt.service.Exception;
using PSW.Lib.Logs;
using System.Linq;
using System.Collections.Generic;

namespace psw.itt.service.Strategies
{
    public class FetchActiveProductCodesList : ApiStrategy<Unspecified, List<FetchActiveProductCodesListResponseDTO>>
    {
        #region Constructors
        public FetchActiveProductCodesList(CommandRequest commandRequest) : base(commandRequest)
        {

        }
        #endregion

        #region Strategy Methods
        public override CommandReply Execute()
        {
            try
            {
                Log.Information("|{0}|{1}| Request DTO {@RequestDTO}", StrategyName, MethodID, RequestDTO);
                var ActiveProductCodesList = Command.UnitOfWork.ProductCodeEntityRepository.GetActiveProductCode();

                ResponseDTO = new List<FetchActiveProductCodesListResponseDTO>();
                ResponseDTO = ActiveProductCodesList.Select(item => Mapper.Map<FetchActiveProductCodesListResponseDTO>(item)).ToList();


                return OKReply("Active Product Code List Fetched Successfully");
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