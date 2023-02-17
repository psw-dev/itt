namespace PSW.ITT.Service.DTO
{
    public struct LPCOFeeCleanResp
    {
        public string CalculationBasis { get; set; }
        public string CalculationSource { get; set; }
        public decimal? Rate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? AdditionalAmount { get; set; }
        public string AdditionalAmountOn { get; set; }
    }
}