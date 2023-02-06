using FluentValidation;
using PSW.ITT.Service.DTO;
using System.Text.RegularExpressions;

namespace PSW.ITT.Service.ModelValidators
{
    public class GetPCTCodeListRequestDTOValidator : AbstractValidator<GetPCTCodeListRequest>
    {
        public GetPCTCodeListRequestDTOValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(c => c.TradeTranTypeID)
            .NotEmpty()
            .NotNull()
            .WithName("tradeTranTypeID")
            .WithMessage("'{PropertyName}' should not be empty.");
            
            
            RuleFor(model => model.HsCode)
            .NotNull()
            .WithMessage("'{PropertyName}' should not be null.")
            .Must(isValidHscode).WithMessage("HS Code is not in correct format.");
            
        }
        protected bool isValidHscode(string HSCode)
        {
            bool isValidHscode = false;
            Regex reg = new Regex(@"([0-9]{4})\.([0-9]{4})");
            isValidHscode = reg.IsMatch(HSCode);
            return isValidHscode;
        }
        
    }
}