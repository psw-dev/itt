using PSW.ITT.Service.DTO;
using PSW.ITT.Data;
using PSW.ITT.Common.Model;
using PSW.ITT.Common.Constants;
using System.Linq;
using PSW.ITT.Common.Enums;
using System.Reflection;
using PSW.Lib.Logs;
using System;

namespace PSW.ITT.service
{
    public class AQDECFeeCalculation
    {
        private AQDECFeeCalculateRequestDTO request { get; set; }
        private ISHRDUnitOfWork shrdUnitOfWork { get; set; }
        private IUnitOfWork unitOfWork { get; set; }
        // public decimal? Amount { get; set; }

        public AQDECFeeCalculation(IUnitOfWork unitOfWork, ISHRDUnitOfWork shrdUnitOfWork, AQDECFeeCalculateRequestDTO request)
        {
            this.unitOfWork = unitOfWork;
            this.shrdUnitOfWork = shrdUnitOfWork;
            this.request = request;
        }

        public SingleResponseModel<AQDECFeeCalculateResponseDTO> CalculateECFee()
        {
            decimal amount = 0;
            // var test = unitOfWork.UV_UnitAQDRepository.Where(new { ID = "100001" }).FirstOrDefault();
            Log.Information("[{0}.{1}] Started. Request: {@RequestDTO}", GetType().Name, MethodBase.GetCurrentMethod().Name, request);
            var response = new SingleResponseModel<AQDECFeeCalculateResponseDTO>();
            var model = new AQDECFeeCalculateResponseDTO();
            var feeConfigurationList = unitOfWork.LPCOFeeStructureRepository.Get();
            // .Where(new
            // {
            //     HSCodeExt = request.HsCodeExt,
            //     Unit_ID = request.AgencyUOMId,
            //     TradeTranTypeID = request.TradeTranTypeID,
            // }).Where(x => x.EffectiveFromDt < DateTime.Now && x.EffectiveThruDt > DateTime.Now);

            if (feeConfigurationList != null && feeConfigurationList.Count() > 0)
            {
                var quantity = request.Quantity;

                var refUnits = shrdUnitOfWork.Ref_UnitsRepository.Where(new
                {
                    Unit_ID = request.AgencyUOMId,
                    IsActive = true
                }).FirstOrDefault();

                if (refUnits != null)
                {
                    if (refUnits.Unit_Code == Ref_Units.PackingUnits)
                    {
                        //Select Fee from list on the basis of quantity
                        var feeConfiguration = feeConfigurationList.Where(d => (double?)d.QtyRangeFrom >= quantity && (double?)d.QtyRangeTo <= quantity).FirstOrDefault();

                        if (feeConfiguration is null) //When quantity exceed max(201) limit
                        {
                            quantity = (quantity - 1) / 100;          // (Quantity - 1) beacause of range 201-300 lie in same block Ex: On 300 Fee should be 275 => (300 -1)/100 = 2 => 2-1 => 1*25+250 = 275
                            quantity = quantity - 1;
                            amount = ((quantity * (int)LPCOConfiguration.IncreasedRate) + 250);
                        }
                        else
                        {
                            amount = feeConfiguration.Rate.GetValueOrDefault();
                        }
                    }
                    else
                    {
                        amount = feeConfigurationList.FirstOrDefault().Rate.GetValueOrDefault() * request.Quantity;
                    }

                    model.Amount = amount.ToString();
                    response.Model = model;
                }
                else
                {
                    var errorMessage = "Unit not found ";
                    response.IsError = true;
                    response.Error = new ErrorResponseModel()
                                         .Category(ErrorCategories.BadRequest)
                                         .Message(errorMessage);
                }
            }
            else
            {
                var errorMessage = "Fee Configuration data not found ";
                response.IsError = true;
                response.Error = new ErrorResponseModel()
                                     .Category(ErrorCategories.BadRequest)
                                     .Message(errorMessage);

            }
            Log.Information("[{0}.{1}] Response {@response}", GetType().Name, MethodBase.GetCurrentMethod().Name, response);
            Log.Information("[{0}.{1}] Ended", GetType().Name, MethodBase.GetCurrentMethod().Name);
            return response;
        }
    }
}