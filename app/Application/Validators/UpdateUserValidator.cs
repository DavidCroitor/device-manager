using Application.DTOs.UserDtos;
using FluentValidation;

namespace Application.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequestDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
            .When(x => x.Name != null);

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .MaximumLength(50).WithMessage("Role must not exceed 50 characters")
            .When(x => x.Role != null);

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(100).WithMessage("Location must not exceed 100 characters.")
            .When(x => x.Location != null);
    }
}
