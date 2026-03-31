using Application.DTOs.UserDtos;
using FluentValidation;

namespace Application.Validators;

public class CreateUserValidator : AbstractValidator<CreateUserRequestDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .MaximumLength(50).WithMessage("Role must not exceed 50 characters");
        
        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(100).WithMessage("Location must not exceed 100 characters.");
    }

}
