using FluentValidation;
using PSW.ITT.Service.DTO;

namespace PSW.ITT.Service.ModelValidators
{
    public class FetchRegulatoryDataAttributeRequestDTOValidator : AbstractValidator<FetchRegulatoryDataAttributeRequestDTO>
    {
        public FetchRegulatoryDataAttributeRequestDTOValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(c => c.AgencyID)
            .NotEmpty()
            .WithName("agencyID")
            .WithMessage("'{PropertyName}' should not be empty.");

            RuleFor(c => c.TradeTranTypeID)
        .NotEmpty()
        .WithName("tradeTranTypeID")
        .WithMessage("'{PropertyName}' should not be empty.");
        }
    }
}