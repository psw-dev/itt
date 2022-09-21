using FluentValidation;
using PSW.ITT.Service.DTO;

namespace PSW.ITT.Service.ModelValidators
{
    public class CloseProductCodeRequestDTOValidator : AbstractValidator<CloseProductCodeRequestDTO>
    {
        public CloseProductCodeRequestDTOValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(c => c.ID)
            .NotEmpty()
            .WithName("id")
            .WithMessage("'{PropertyName}' should not be empty.");
        }
    }
}