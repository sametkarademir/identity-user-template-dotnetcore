using FluentValidation;

namespace Application.Dtos.Users;

public class UpdateAppUserRequestDto
{
    public string? PhoneNumber { get; set; }
}

public class UpdateAppUserRequestValidator : AbstractValidator<UpdateAppUserRequestDto>
{
    public UpdateAppUserRequestValidator()
    {
        RuleFor(x => x.PhoneNumber).NotEmpty().MinimumLength(10).MaximumLength(10);
    }
}