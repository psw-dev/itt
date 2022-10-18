using FluentValidation;
using PSW.ITT.Service.DTO;

namespace PSW.ITT.Service.ModelValidators
{
    public class DeleteRegulatoryDataRequestDTOValidator : AbstractValidator<DeleteRegulatoryDataRequestDTO>
    {
        public DeleteRegulatoryDataRequestDTOValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(c => c.ID)
            .NotEmpty()
            .WithName("id")
            .WithMessage("'{PropertyName}' should not be empty.");
        }
    }
}