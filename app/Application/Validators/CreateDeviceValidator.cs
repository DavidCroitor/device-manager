using Application.DTOs.DeviceDtos;
using FluentValidation;

namespace Application.Validators;

public class CreateDeviceValidator : AbstractValidator<CreateDeviceRequestDto>
{
    public CreateDeviceValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Device name is required.")
            .MinimumLength(2).WithMessage("Device name must be at least 2 characters long.")
            .MaximumLength(100).WithMessage("Device name must not exceed 100 characters.");

        RuleFor(x => x.Manufacturer)
            .NotEmpty().WithMessage("Manufacturer is required.")
            .MaximumLength(100).WithMessage("Manufacturer must not exceed 100 characters.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Device type is required.")
            .Must(type => type.ToLower() == "phone" || type.ToLower() == "tablet").WithMessage("Type must be 'phone' or 'tablet'.");

        RuleFor(x => x.OS)
            .NotEmpty().WithMessage("OS is required.")
            .MaximumLength(50).WithMessage("OS must not exceed 50 characters");
        
        RuleFor(x => x.OSVersion)
            .NotEmpty().WithMessage("OS version is required.")
            .MaximumLength(50).WithMessage("OS version must not exceed 50 characters");

        RuleFor(x => x.Processor)
            .NotEmpty().WithMessage("Processor is required.")
            .MaximumLength(100).WithMessage("Processor must not exceed 100 characters");

        RuleFor(x => x.RamGB)
            .GreaterThan(0).WithMessage("RAM must be greater than 0");

    }
}
