using FluentValidation;
using PSW.ITT.Service.DTO;

namespace PSW.ITT.Service.ModelValidators
{
    public class ExtendProductCodeRequestDTOValidator : AbstractValidator<EditProductCodeRequestDTO>
    {
        public ExtendProductCodeRequestDTOValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(c => c.EffectiveThruDt)
            .NotNull()
            .WithName("effectiveThruDt")
            .WithMessage("'{PropertyName}' should not be null.");


        }
    }
}